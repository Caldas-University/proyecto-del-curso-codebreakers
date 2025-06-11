using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;

namespace SponsorshipManagement.API.Controllers
{
    /// <summary>
    /// Controlador para consulta avanzada de compromisos
    /// 🎯 CU-PA-02.04.1: Servicio para consultar compromisos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CommitmentQueryController : ControllerBase
    {
        private readonly ICommitmentQueryService _queryService;

        public CommitmentQueryController(ICommitmentQueryService queryService)
        {
            _queryService = queryService;
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Obtiene compromisos con filtros avanzados
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <param name="status">Estados a filtrar (opcional)</param>
        /// <param name="responsible">Responsable (opcional)</param>
        /// <param name="onlyOverdue">Solo vencidos</param>
        /// <param name="onlyNearExpiry">Solo próximos a vencer</param>
        /// <returns>Compromisos filtrados</returns>
        [HttpGet("contract/{contractId}")]
        [ProducesResponseType(typeof(IEnumerable<CommitmentQueryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CommitmentQueryDto>>> GetCommitmentsWithFilters(
            Guid contractId,
            [FromQuery] string[]? status = null,
            [FromQuery] string? responsible = null,
            [FromQuery] bool onlyOverdue = false,
            [FromQuery] bool onlyNearExpiry = false)
        {
            try
            {
                var filters = new CommitmentQueryFilters
                {
                    ContractId = contractId,
                    ResponsibleFilter = responsible,
                    OnlyOverdue = onlyOverdue,
                    OnlyNearExpiry = onlyNearExpiry
                };

                // Parsear estados si se proporcionan
                if (status != null && status.Length > 0)
                {
                    foreach (var s in status)
                    {
                        if (Enum.TryParse<Domain.Entities.CommitmentStatus>(s, true, out var statusEnum))
                        {
                            filters.StatusFilter.Add(statusEnum);
                        }
                    }
                }

                var commitments = await _queryService.GetCommitmentsWithFiltersAsync(filters);

                if (!commitments.Any())
                {
                    return NotFound($"No se encontraron compromisos para el contrato {contractId} con los filtros especificados");
                }

                return Ok(commitments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Obtiene compromisos agrupados por estado
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Compromisos agrupados por estado</returns>
        [HttpGet("contract/{contractId}/grouped")]
        [ProducesResponseType(typeof(Dictionary<string, List<CommitmentQueryDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, List<CommitmentQueryDto>>>> GetCommitmentsGroupedByStatus(Guid contractId)
        {
            try
            {
                var groupedCommitments = await _queryService.GetCommitmentsGroupedByStatusAsync(contractId);

                if (!groupedCommitments.Any())
                {
                    return Ok(new Dictionary<string, List<CommitmentQueryDto>>
                    {
                        ["message"] = new List<CommitmentQueryDto>()
                    });
                }

                return Ok(groupedCommitments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Obtiene compromisos críticos
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Compromisos críticos</returns>
        [HttpGet("contract/{contractId}/critical")]
        [ProducesResponseType(typeof(IEnumerable<CommitmentQueryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommitmentQueryDto>>> GetCriticalCommitments(Guid contractId)
        {
            try
            {
                var criticalCommitments = await _queryService.GetCriticalCommitmentsAsync(contractId);
                return Ok(criticalCommitments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Busca compromisos por texto
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <param name="q">Texto a buscar</param>
        /// <returns>Compromisos que coinciden con la búsqueda</returns>
        [HttpGet("contract/{contractId}/search")]
        [ProducesResponseType(typeof(IEnumerable<CommitmentQueryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommitmentQueryDto>>> SearchCommitments(Guid contractId, [FromQuery] string q)
        {
            try
            {
                var searchResults = await _queryService.SearchCommitmentsAsync(contractId, q);
                return Ok(searchResults);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Obtiene historial de cambios de estado
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Historial de cambios</returns>
        [HttpGet("contract/{contractId}/history")]
        [ProducesResponseType(typeof(IEnumerable<CommitmentStatusHistoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommitmentStatusHistoryDto>>> GetStatusHistory(Guid contractId)
        {
            try
            {
                var history = await _queryService.GetStatusHistoryAsync(contractId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
