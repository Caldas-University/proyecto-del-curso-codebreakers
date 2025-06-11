using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IContractComplianceService
    {
        Task<ContractComplianceReportDto> ValidateContractComplianceAsync(Guid contractId);
        Task<ContractComplianceReportDto> GenerateComplianceReportAsync(Guid contractId, string createdBy);
        Task<IEnumerable<ContractComplianceReportDto>> GetComplianceHistoryAsync(Guid contractId);
        Task<ContractComplianceReportDto?> GetLatestComplianceReportAsync(Guid contractId);
        Task<IEnumerable<ContractComplianceReportDto>> GetContractsAtRiskAsync();
        Task<IEnumerable<ContractCommitmentDto>> GetOverdueCommitmentsAsync();
        Task<bool> UpdateCommitmentProgressAsync(Guid commitmentId, decimal newValue, string notes = "");
        Task<string> GetRecommendationsAsync(Guid contractId);
    }
}