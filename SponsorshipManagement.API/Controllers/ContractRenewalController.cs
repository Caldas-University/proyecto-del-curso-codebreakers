using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de renovaciones y finalización de contratos
    /// 🎯 CU-PA-05: Gestionar renovaciones y finalización de contratos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContractRenewalController : ControllerBase
    {
        private readonly IContractRenewalService _renewalService;
        private readonly INotificationService _notificationService;

        public ContractRenewalController(
            IContractRenewalService renewalService,
            INotificationService notificationService)
        {
            _renewalService = renewalService;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Obtiene contratos que vencen en los próximos N días
        /// 🎯 CU-PA-05.01.1 y CU-PA-05.01.3: Consulta y exposición de contratos próximos a vencer
        /// </summary>
        /// <param name="days">Número de días para considerar (por defecto 30)</param>
        /// <param name="includeStatuses">Estados de contrato a incluir (opcional)</param>
        /// <param name="sponsorId">ID del patrocinador para filtrar (opcional)</param>
        /// <param name="eventId">ID del evento para filtrar (opcional)</param>
        /// <param name="minValue">Valor mínimo del contrato (opcional)</param>
        /// <param name="notified">Filtrar por contratos ya notificados (opcional)</param>
        /// <returns>Lista de contratos próximos a vencer</returns>
        [HttpGet("expiring")]
        [ProducesResponseType(typeof(IEnumerable<ContractRenewalDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ContractRenewalDto>>> GetExpiringContracts(
            [FromQuery] int days = 30,
            [FromQuery] string[]? includeStatuses = null,
            [FromQuery] string? sponsorId = null,
            [FromQuery] string? eventId = null,
            [FromQuery] decimal? minValue = null,
            [FromQuery] bool? notified = null)
        {
            try
            {
                Console.WriteLine($"📥 CU-PA-05.01.3: Endpoint expiring invocado con days={days}");
                
                if (includeStatuses == null || includeStatuses.Length == 0)
                {
                    includeStatuses = new[] { "Active", "Signed" };
                }
                
                Console.WriteLine($"🔍 Parámetros de filtrado: estados={string.Join(",", includeStatuses)}, " +
                                  $"sponsorId={sponsorId}, eventId={eventId}, minValue={minValue}, notified={notified}");
                
                var expiringFilter = new ExpiringContractsFilterDto
                {
                    Days = days,
                    IncludeStatuses = includeStatuses,
                    SponsorId = sponsorId,
                    EventId = eventId,
                    MinimumContractValue = minValue,  // Corregido: MinValue -> MinimumContractValue
                    NotificationSent = notified       // Corregido: NotificationStatus -> NotificationSent
                };
                
                var contracts = await _renewalService.GetContractsExpiringAsync(expiringFilter);
                
                Console.WriteLine($"✅ CU-PA-05.01.3: Encontrados {contracts.Count()} contratos próximos a vencer");
                
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR EN ENDPOINT: {ex.Message}");
                Console.WriteLine($"❌ STACK TRACE: {ex.StackTrace}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un resumen estadístico de los contratos próximos a vencer
        /// 🎯 CU-PA-05.01.3: Exposición de estadísticas de contratos próximos a vencer
        /// </summary>
        /// <param name="days">Número de días para considerar (por defecto 30)</param>
        /// <returns>Resumen estadístico de contratos próximos a vencer</returns>
        [HttpGet("expiring-summary")]
        [ProducesResponseType(typeof(ExpiringContractsSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExpiringContractsSummaryDto>> GetExpiringContractsSummary(
            [FromQuery] int days = 30)
        {
            try
            {
                Console.WriteLine($"📥 CU-PA-05.01.3: Endpoint expiring-summary invocado con days={days}");
                
                var summary = await _renewalService.GetExpiringContractsSummaryAsync(days);
                
                Console.WriteLine($"✅ CU-PA-05.01.3: Generado resumen para {summary.TotalExpiringContracts} contratos");
                
                return Ok(summary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR EN ENDPOINT: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Marca un contrato como notificado para renovación
        /// </summary>
        /// <param name="id">ID del contrato</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("{id}/notify")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> MarkAsNotified(Guid id)
        {
            try
            {
                var success = await _renewalService.MarkContractAsNotifiedAsync(id);
                
                if (!success)
                    return NotFound($"Contrato con ID {id} no encontrado");
                
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error marcando contrato como notificado: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida el cumplimiento e impacto publicitario para la renovación de un contrato
        /// 🎯 CU-PA-05.01.2: Validación de cumplimiento e impacto publicitario
        /// </summary>
        /// <param name="id">ID del contrato a validar</param>
        /// <returns>Datos de validación para la renovación</returns>
        [HttpGet("validate/{id}")]
        [ProducesResponseType(typeof(ContractRenewalValidationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ContractRenewalValidationDto>> ValidateContractForRenewal(Guid id)
        {
            try
            {
                Console.WriteLine($"📥 Recibiendo solicitud de validación para contrato {id}");
                
                var validationData = await _renewalService.ValidateContractForRenewalAsync(id);
                
                Console.WriteLine($"✅ Validación generada para contrato {id} con score {validationData.OverallScore}/100");
                
                return Ok(validationData);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"❌ Contrato no encontrado: {ex.Message}");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en validación: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Genera un mensaje estructurado para un contrato
        /// 🎯 CU-PA-05.02.2: Mensajes estructurados según rol
        /// </summary>
        [HttpGet("{id}/message")]
        [ProducesResponseType(typeof(ContractActionMessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ContractActionMessageDto>> GenerateMessage(
            Guid id, 
            [FromQuery] string role = "Patrocinador", 
            [FromQuery] string email = "test@example.com")
        {
            try
            {
                Console.WriteLine($"📥 CU-PA-05.02.2: Generando mensaje para contrato {id}, rol={role}, email={email}");
                
                if (string.IsNullOrEmpty(role))
                {
                    role = "Patrocinador";
                }
                
                if (string.IsNullOrEmpty(email))
                {
                    email = "test@example.com";
                }
                
                // Verificar que el contrato existe
                var contract = await _renewalService.GetContractByIdAsync(id);
                if (contract == null)
                {
                    return NotFound($"Contrato con ID {id} no encontrado");
                }
                
                var message = await _notificationService.GenerateActionMessageAsync(id.ToString(), role, email);
                
                Console.WriteLine($"✅ CU-PA-05.02.2: Mensaje generado con {message.RenewalOptions.Count} opciones de renovación");
                
                return Ok(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error generando mensaje: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Envía notificación estructurada para un contrato
        /// 🎯 CU-PA-05.02.2: Envío de mensajes estructurados
        /// </summary>
        [HttpPost("{id}/send-structured-notification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> SendStructuredNotification(
            Guid id, 
            [FromQuery] string role = "Patrocinador",
            [FromQuery] string email = "test@example.com")
        {
            try
            {
                Console.WriteLine($"📥 CU-PA-05.02.2: Enviando notificación para contrato {id}, rol={role}, email={email}");
                
                if (string.IsNullOrEmpty(role))
                {
                    role = "Patrocinador";
                }
                
                if (string.IsNullOrEmpty(email))
                {
                    email = "test@example.com";
                }
                
                // Verificar que el contrato existe
                var contract = await _renewalService.GetContractByIdAsync(id);
                if (contract == null)
                {
                    return NotFound($"Contrato con ID {id} no encontrado");
                }
                
                // Generar el mensaje
                var message = await _notificationService.GenerateActionMessageAsync(id.ToString(), role, email);
                
                // Enviar el mensaje
                bool success = await _notificationService.SendActionMessageAsync(message);
                
                // Marcar el contrato como notificado (opcional)
                if (success)
                {
                    await _renewalService.MarkContractAsNotifiedAsync(id);
                }
                
                Console.WriteLine($"✅ CU-PA-05.02.2: Notificación enviada correctamente");
                
                return Ok(new { success = true, message = "Notificación enviada correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error enviando notificación: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Registra una decisión de renovación o finalización de contrato
        /// 🎯 CU-PA-05.02.3: Backend para registrar decisiones de renovación o finalización
        /// </summary>
        /// <param name="decision">Datos de la decisión</param>
        /// <returns>Resultado del registro de la decisión</returns>
        [HttpPost("register-decision")]
        [ProducesResponseType(typeof(ContractDecisionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ContractDecisionResponseDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ContractDecisionResponseDto>> RegisterContractDecision(
            [FromBody] ContractDecisionDto decision)
        {
            try
            {
                Console.WriteLine($"📥 CU-PA-05.02.3: Endpoint register-decision invocado para contrato {decision.ContractId}");
                
                // Validaciones básicas
                if (string.IsNullOrEmpty(decision.ContractId))
                {
                    return BadRequest(new ContractDecisionResponseDto
                    {
                        Success = false,
                        Message = "El ID del contrato es requerido"
                    });
                }
                
                if (string.IsNullOrEmpty(decision.DecisionType))
                {
                    return BadRequest(new ContractDecisionResponseDto
                    {
                        Success = false,
                        Message = "El tipo de decisión es requerido"
                    });
                }
                
                if (string.IsNullOrEmpty(decision.UserEmail) || string.IsNullOrEmpty(decision.UserRole))
                {
                    return BadRequest(new ContractDecisionResponseDto
                    {
                        Success = false,
                        Message = "El email y rol del usuario son requeridos"
                    });
                }
                
                // Registrar la decisión
                var result = await _renewalService.RegisterContractDecisionAsync(decision);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                
                Console.WriteLine($"✅ CU-PA-05.02.3: Decisión registrada correctamente con ID {result.DecisionId}");
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en endpoint register-decision: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}