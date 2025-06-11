using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;

namespace SponsorshipManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractComplianceController : ControllerBase
    {
        private readonly IContractComplianceService _complianceService;

        public ContractComplianceController(IContractComplianceService complianceService)
        {
            _complianceService = complianceService;
        }

        /// <summary>
        /// Valida el cumplimiento parcial de metas de un contrato
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Reporte de cumplimiento actualizado</returns>
        [HttpGet("validate/{contractId}")]
        public async Task<ActionResult<ContractComplianceReportDto>> ValidateContractCompliance(Guid contractId)
        {
            try
            {
                var report = await _complianceService.ValidateContractComplianceAsync(contractId);
                return Ok(report);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Genera y guarda un reporte de cumplimiento completo
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <param name="createdBy">Usuario que genera el reporte</param>
        /// <returns>Reporte de cumplimiento guardado</returns>
        [HttpPost("generate-report/{contractId}")]
        public async Task<ActionResult<ContractComplianceReportDto>> GenerateComplianceReport(Guid contractId, [FromQuery] string createdBy = "system")
        {
            try
            {
                var report = await _complianceService.GenerateComplianceReportAsync(contractId, createdBy);
                return CreatedAtAction(nameof(GetLatestComplianceReport), new { contractId }, report);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el historial completo de cumplimiento de un contrato
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Lista histórica de reportes de cumplimiento</returns>
        [HttpGet("history/{contractId}")]
        public async Task<ActionResult<IEnumerable<ContractComplianceReportDto>>> GetComplianceHistory(Guid contractId)
        {
            var reports = await _complianceService.GetComplianceHistoryAsync(contractId);
            return Ok(reports);
        }

        /// <summary>
        /// Obtiene el último reporte de cumplimiento de un contrato
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Último reporte de cumplimiento disponible</returns>
        [HttpGet("latest/{contractId}")]
        public async Task<ActionResult<ContractComplianceReportDto>> GetLatestComplianceReport(Guid contractId)
        {
            var report = await _complianceService.GetLatestComplianceReportAsync(contractId);
            if (report == null)
                return NotFound($"No se encontraron reportes de cumplimiento para el contrato {contractId}");
            
            return Ok(report);
        }

        /// <summary>
        /// Obtiene todos los contratos que están en riesgo de incumplimiento
        /// </summary>
        /// <returns>Lista de contratos en riesgo</returns>
        [HttpGet("contracts-at-risk")]
        public async Task<ActionResult<IEnumerable<ContractComplianceReportDto>>> GetContractsAtRisk()
        {
            var reports = await _complianceService.GetContractsAtRiskAsync();
            return Ok(reports);
        }

        /// <summary>
        /// Obtiene todos los compromisos que están vencidos
        /// </summary>
        /// <returns>Lista de compromisos vencidos</returns>
        [HttpGet("overdue-commitments")]
        public async Task<ActionResult<IEnumerable<ContractCommitmentDto>>> GetOverdueCommitments()
        {
            var commitments = await _complianceService.GetOverdueCommitmentsAsync();
            return Ok(commitments);
        }

        /// <summary>
        /// Actualiza el progreso de un compromiso específico
        /// </summary>
        /// <param name="commitmentId">ID del compromiso</param>
        /// <param name="request">Datos de actualización del progreso</param>
        /// <returns>Resultado de la actualización</returns>
        [HttpPut("commitment/{commitmentId}/progress")]
        public async Task<ActionResult> UpdateCommitmentProgress(Guid commitmentId, [FromBody] UpdateCommitmentProgressRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _complianceService.UpdateCommitmentProgressAsync(commitmentId, request.NewValue, request.Notes);
            if (updated)
                return NoContent();
            
            return NotFound($"Compromiso con ID {commitmentId} no encontrado");
        }

        /// <summary>
        /// Obtiene recomendaciones específicas para mejorar el cumplimiento de un contrato
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Recomendaciones de mejora</returns>
        [HttpGet("recommendations/{contractId}")]
        public async Task<ActionResult> GetRecommendations(Guid contractId)
        {
            try
            {
                var recommendations = await _complianceService.GetRecommendationsAsync(contractId);
                return Ok(new { 
                    ContractId = contractId, 
                    Recommendations = recommendations,
                    GeneratedAt = DateTime.UtcNow
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene resumen ejecutivo de cumplimiento para dashboard
        /// </summary>
        /// <returns>Resumen de cumplimiento de todos los contratos</returns>
        [HttpGet("dashboard-summary")]
        public async Task<ActionResult> GetDashboardSummary()
        {
            var contractsAtRisk = await _complianceService.GetContractsAtRiskAsync();
            var overdueCommitments = await _complianceService.GetOverdueCommitmentsAsync();

            var summary = new
            {
                ContractsAtRisk = contractsAtRisk.Count(),
                OverdueCommitments = overdueCommitments.Count(),
                CriticalContracts = contractsAtRisk.Count(c => c.RiskLevel == "Critical"),
                GeneratedAt = DateTime.UtcNow
            };

            return Ok(summary);
        }
    }

    // DTO para actualizar progreso
    public class UpdateCommitmentProgressRequest
    {
        public decimal NewValue { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}