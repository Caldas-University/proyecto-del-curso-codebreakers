using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class ContractComplianceService : IContractComplianceService
    {
        private readonly IContractRepository _contractRepository;
        private readonly IContractCommitmentRepository _commitmentRepository;
        private readonly IContractComplianceReportRepository _reportRepository;

        public ContractComplianceService(
            IContractRepository contractRepository,
            IContractCommitmentRepository commitmentRepository,
            IContractComplianceReportRepository reportRepository)
        {
            _contractRepository = contractRepository;
            _commitmentRepository = commitmentRepository;
            _reportRepository = reportRepository;
        }

        public async Task<ContractComplianceReportDto> ValidateContractComplianceAsync(Guid contractId)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
                throw new ArgumentException($"Contrato con ID {contractId} no encontrado");

            var commitments = await _commitmentRepository.GetByContractIdAsync(contractId);
            var report = GenerateComplianceReport(contract, commitments.ToList());
            
            return MapReportToDto(report);
        }

        public async Task<ContractComplianceReportDto> GenerateComplianceReportAsync(Guid contractId, string createdBy)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
                throw new ArgumentException($"Contrato con ID {contractId} no encontrado");

            var commitments = await _commitmentRepository.GetByContractIdAsync(contractId);
            var report = GenerateComplianceReport(contract, commitments.ToList());
            report.CreatedBy = createdBy;

            var savedReport = await _reportRepository.CreateAsync(report);
            return MapReportToDto(savedReport);
        }

        public async Task<IEnumerable<ContractComplianceReportDto>> GetComplianceHistoryAsync(Guid contractId)
        {
            var reports = await _reportRepository.GetByContractIdAsync(contractId);
            return reports.Select(MapReportToDto).OrderByDescending(r => r.ReportDate);
        }

        public async Task<ContractComplianceReportDto?> GetLatestComplianceReportAsync(Guid contractId)
        {
            var report = await _reportRepository.GetLatestByContractIdAsync(contractId);
            return report != null ? MapReportToDto(report) : null;
        }

        public async Task<IEnumerable<ContractComplianceReportDto>> GetContractsAtRiskAsync()
        {
            var allReports = await _reportRepository.GetAllAsync();
            var atRiskReports = allReports.Where(r => 
                r.RiskLevel == RiskLevel.High || 
                r.RiskLevel == RiskLevel.Critical ||
                r.OverallCompliancePercentage < 60);
            
            return atRiskReports.Select(MapReportToDto);
        }

        public async Task<IEnumerable<ContractCommitmentDto>> GetOverdueCommitmentsAsync()
        {
            var overdueCommitments = await _commitmentRepository.GetOverdueCommitmentsAsync();
            return overdueCommitments.Select(MapCommitmentToDto);
        }

        public async Task<bool> UpdateCommitmentProgressAsync(Guid commitmentId, decimal newValue, string notes = "")
        {
            var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
            if (commitment == null)
                return false;

            commitment.CurrentValue = newValue;
            commitment.UpdatedAt = DateTime.UtcNow;
            commitment.Notes = notes;

            // Actualizar estado si se completó
            if (commitment.GetCompliancePercentage() >= 100 && commitment.Status != CommitmentStatus.Completed)
            {
                commitment.Status = CommitmentStatus.Completed;
                commitment.CompletedAt = DateTime.UtcNow;
            }
            else if (commitment.Status == CommitmentStatus.Pending && newValue > 0)
            {
                commitment.Status = CommitmentStatus.InProgress;
            }

            await _commitmentRepository.UpdateAsync(commitment);
            return true;
        }

        public Task<string> GetRecommendationsAsync(Guid contractId)
        {
            var reportTask = ValidateContractComplianceAsync(contractId);
            // Esperar el resultado de la tarea (sincrónicamente, ya que no hay await real)
            var report = reportTask.GetAwaiter().GetResult();
            return Task.FromResult(GenerateRecommendations(report));
        }

        private ContractComplianceReport GenerateComplianceReport(Contract contract, List<ContractCommitment> commitments)
        {
            var report = new ContractComplianceReport
            {
                ContractId = contract.Id,
                TotalCommitments = commitments.Count
            };

            var commitmentCompliances = new List<CommitmentCompliance>();

            foreach (var commitment in commitments)
            {
                var compliance = new CommitmentCompliance
                {
                    CommitmentId = commitment.Id,
                    Title = commitment.Title,
                    TargetValue = commitment.TargetValue,
                    CurrentValue = commitment.CurrentValue,
                    CompliancePercentage = commitment.GetCompliancePercentage(),
                    Status = commitment.Status,
                    DaysRemaining = Math.Max((commitment.TargetDate - DateTime.UtcNow).Days, 0),
                    RiskLevel = DetermineRiskLevel(commitment),
                    ActionRequired = DetermineActionRequired(commitment)
                };

                commitmentCompliances.Add(compliance);

                // Contar por estado
                switch (commitment.Status)
                {
                    case CommitmentStatus.Completed:
                        report.CompletedCommitments++;
                        break;
                    case CommitmentStatus.InProgress:
                        report.InProgressCommitments++;
                        break;
                    case CommitmentStatus.Overdue:
                        report.OverdueCommitments++;
                        break;
                }

                if (commitment.IsOverdue())
                    report.OverdueCommitments++;
            }

            report.CommitmentCompliances = commitmentCompliances;
            report.OverallCompliancePercentage = commitments.Count > 0 
                ? commitments.Average(c => c.GetCompliancePercentage()) 
                : 0;

            report.OverallStatus = DetermineOverallStatus(report.OverallCompliancePercentage);
            report.RiskLevel = DetermineOverallRiskLevel(report, commitmentCompliances);
            report.Recommendations = GenerateRecommendationsFromReport(report);

            return report;
        }

        private RiskLevel DetermineRiskLevel(ContractCommitment commitment)
        {
            var compliancePercentage = commitment.GetCompliancePercentage();
            var daysRemaining = (commitment.TargetDate - DateTime.UtcNow).Days;

            if (commitment.IsOverdue())
                return RiskLevel.Critical;

            if (daysRemaining <= 3 && compliancePercentage < 70)
                return RiskLevel.High;

            if (daysRemaining <= 7 && compliancePercentage < 50)
                return RiskLevel.High;

            if (compliancePercentage < 30)
                return RiskLevel.Medium;

            return RiskLevel.Low;
        }

        private string DetermineActionRequired(ContractCommitment commitment)
        {
            if (commitment.Status == CommitmentStatus.Completed)
                return "Meta completada exitosamente";

            if (commitment.IsOverdue())
                return "URGENTE: Meta vencida - Revisar estrategia inmediatamente";

            var daysRemaining = (commitment.TargetDate - DateTime.UtcNow).Days;
            var compliancePercentage = commitment.GetCompliancePercentage();

            if (daysRemaining <= 3 && compliancePercentage < 80)
                return "CRÍTICO: Acelerar ejecución inmediatamente";

            if (daysRemaining <= 7 && compliancePercentage < 60)
                return "ALTO: Intensificar esfuerzos esta semana";

            if (compliancePercentage < 30)
                return "MEDIO: Revisar y ajustar estrategia";

            return "BAJO: Continuar según el plan establecido";
        }

        private ComplianceStatus DetermineOverallStatus(decimal percentage)
        {
            return percentage switch
            {
                >= 90 => ComplianceStatus.Excellent,
                >= 75 => ComplianceStatus.Good,
                >= 50 => ComplianceStatus.Regular,
                _ => ComplianceStatus.Poor
            };
        }

        private RiskLevel DetermineOverallRiskLevel(ContractComplianceReport report, List<CommitmentCompliance> compliances)
        {
            var criticalCount = compliances.Count(c => c.RiskLevel == RiskLevel.Critical);
            var highCount = compliances.Count(c => c.RiskLevel == RiskLevel.High);

            if (criticalCount > 0 || report.OverallCompliancePercentage < 30)
                return RiskLevel.Critical;

            if (highCount > 0 || report.OverallCompliancePercentage < 60)
                return RiskLevel.High;

            if (report.OverallCompliancePercentage < 80)
                return RiskLevel.Medium;

            return RiskLevel.Low;
        }

        private string GenerateRecommendationsFromReport(ContractComplianceReport report)
        {
            var recommendations = new List<string>();

            if (report.OverallCompliancePercentage < 50)
                recommendations.Add("Revisar estrategia general del contrato y recursos asignados");

            if (report.OverdueCommitments > 0)
                recommendations.Add($"Atender inmediatamente {report.OverdueCommitments} compromiso(s) vencido(s)");

            var criticalCommitments = report.CommitmentCompliances.Count(c => c.RiskLevel == RiskLevel.Critical);
            if (criticalCommitments > 0)
                recommendations.Add($"Priorizar {criticalCommitments} compromiso(s) crítico(s)");

            var highRiskCommitments = report.CommitmentCompliances.Count(c => c.RiskLevel == RiskLevel.High);
            if (highRiskCommitments > 0)
                recommendations.Add($"Acelerar ejecución de {highRiskCommitments} compromiso(s) de alto riesgo");

            if (report.InProgressCommitments > report.CompletedCommitments && report.OverallCompliancePercentage < 70)
                recommendations.Add("Considerar recursos adicionales para acelerar el progreso");

            return recommendations.Count > 0 ? string.Join("; ", recommendations) : "Mantener el ritmo actual de trabajo";
        }

        private string GenerateRecommendations(ContractComplianceReportDto report)
        {
            return report.Recommendations;
        }

        private ContractComplianceReportDto MapReportToDto(ContractComplianceReport report)
        {
            return new ContractComplianceReportDto
            {
                Id = report.Id,
                ContractId = report.ContractId,
                ReportDate = report.ReportDate,
                OverallCompliancePercentage = report.OverallCompliancePercentage,
                OverallStatus = report.OverallStatus.ToString(),
                TotalCommitments = report.TotalCommitments,
                CompletedCommitments = report.CompletedCommitments,
                InProgressCommitments = report.InProgressCommitments,
                OverdueCommitments = report.OverdueCommitments,
                CommitmentCompliances = report.CommitmentCompliances.Select(c => new CommitmentComplianceDto
                {
                    CommitmentId = c.CommitmentId,
                    Title = c.Title,
                    TargetValue = c.TargetValue,
                    CurrentValue = c.CurrentValue,
                    CompliancePercentage = c.CompliancePercentage,
                    Status = c.Status.ToString(),
                    DaysRemaining = c.DaysRemaining,
                    RiskLevel = c.RiskLevel.ToString(),
                    ActionRequired = c.ActionRequired
                }).ToList(),
                Recommendations = report.Recommendations,
                RiskLevel = report.RiskLevel.ToString(),
                CreatedAt = report.CreatedAt,
                CreatedBy = report.CreatedBy
            };
        }

        private ContractCommitmentDto MapCommitmentToDto(ContractCommitment commitment)
        {
            return new ContractCommitmentDto
            {
                Id = commitment.Id,
                ContractId = commitment.ContractId,
                Title = commitment.Title,
                Description = commitment.Description,
                Type = commitment.Type.ToString(),
                TargetValue = commitment.TargetValue,
                CurrentValue = commitment.CurrentValue,
                Unit = commitment.Unit,
                TargetDate = commitment.TargetDate,
                Priority = commitment.Priority.ToString(),
                Status = commitment.Status.ToString(),
                CompletedAt = commitment.CompletedAt,
                Notes = commitment.Notes,
                CreatedAt = commitment.CreatedAt,
                CompliancePercentage = commitment.GetCompliancePercentage(),
                IsOverdue = commitment.IsOverdue(),
                IsAtRisk = commitment.IsAtRisk()
            };
        }
    }
}