using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Services;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de estados de compromisos
    /// 🎯 CU-PA-02.01.4: Asignación y transiciones de estado
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CommitmentStateController : ControllerBase
    {
        private readonly CommitmentStateService _stateService;

        public CommitmentStateController(CommitmentStateService stateService)
        {
            _stateService = stateService;
        }

        /// <summary>
        /// 🎯 CU-PA-02.01.4: Cambia el estado de un compromiso
        /// </summary>
        /// <param name="id">ID del compromiso</param>
        /// <param name="request">Datos del cambio de estado</param>
        /// <returns>Resultado del cambio de estado</returns>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ChangeCommitmentStatus(Guid id, [FromBody] ChangeStatusRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Los datos del cambio de estado son requeridos");
                }

                if (!Enum.TryParse<CommitmentStatus>(request.NewStatus, true, out var newStatus))
                {
                    return BadRequest($"Estado inválido: {request.NewStatus}");
                }

                Console.WriteLine($"🔄 Cambiando estado del compromiso {id} a {newStatus}");

                // Validar transición
                var isValidTransition = await _stateService.ValidateStateTransitionAsync(id, newStatus);
                if (!isValidTransition)
                {
                    return BadRequest($"Transición de estado inválida hacia {newStatus}");
                }

                // 🔧 FIX CS8604: Asegurar que reason no sea null
                var reason = request.Reason ?? string.Empty;

                // Cambiar estado
                var success = await _stateService.ChangeCommitmentStatusAsync(id, newStatus, reason);
                
                if (!success)
                {
                    return NotFound($"No se pudo cambiar el estado del compromiso {id}");
                }

                return Ok($"Estado del compromiso {id} cambiado exitosamente a {newStatus}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error cambiando estado: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Procesa compromisos vencidos automáticamente
        /// </summary>
        /// <returns>Número de compromisos procesados</returns>
        [HttpPost("process-overdue")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult> ProcessOverdueCommitments()
        {
            try
            {
                var processedCount = await _stateService.ProcessOverdueCommitmentsAsync();
                
                return Ok(new 
                { 
                    message = "Compromisos vencidos procesados exitosamente",
                    processedCount = processedCount,
                    processedAt = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error procesando compromisos vencidos: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene estadísticas de estados de compromisos
        /// </summary>
        /// <returns>Estadísticas detalladas</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(StateStatistics), StatusCodes.Status200OK)]
        public async Task<ActionResult<StateStatistics>> GetStateStatistics()
        {
            try
            {
                var statistics = await _stateService.GetStateStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo estadísticas: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.01.4: Obtiene los estados válidos desde un estado actual
        /// </summary>
        /// <param name="currentStatus">Estado actual</param>
        /// <returns>Lista de estados válidos</returns>
        [HttpGet("valid-transitions/{currentStatus}")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        public ActionResult<string[]> GetValidTransitions(string currentStatus)
        {
            try
            {
                if (!Enum.TryParse<CommitmentStatus>(currentStatus, true, out var status))
                {
                    return BadRequest($"Estado inválido: {currentStatus}");
                }

                var validTransitions = GetValidTransitionsForStatus(status);
                return Ok(validTransitions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo transiciones válidas: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene las transiciones válidas para un estado
        /// </summary>
        private static string[] GetValidTransitionsForStatus(CommitmentStatus status)
        {
            return status switch
            {
                CommitmentStatus.Pending => new[] { "InProgress", "Cancelled", "Overdue" },
                CommitmentStatus.InProgress => new[] { "Completed", "OnHold", "Cancelled" },
                CommitmentStatus.OnHold => new[] { "InProgress", "Cancelled" },
                CommitmentStatus.Overdue => new[] { "InProgress", "Cancelled" },
                CommitmentStatus.Completed => new string[0], // No se puede cambiar
                CommitmentStatus.Cancelled => new string[0], // No se puede cambiar
                _ => new string[0]
            };
        }
    }

    /// <summary>
    /// Request para cambio de estado
    /// </summary>
    public class ChangeStatusRequest
    {
        public string NewStatus { get; set; } = string.Empty;
        public string? Reason { get; set; } = string.Empty;
    }
}