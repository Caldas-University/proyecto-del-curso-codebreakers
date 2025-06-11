using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Application.Services;    // Para AuditStatistics
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using IAuditService = SponsorshipManagement.Application.Interfaces.IAuditService;

namespace SponsorshipManagement.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de auditoría
    /// CU-PA-02.01.5: Registro de auditoría al crear compromiso
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;  // USAR LA INTERFAZ CORRECTA
        private readonly IAuditRepository _auditRepository;

        public AuditController(IAuditService auditService, IAuditRepository auditRepository)
        {
            _auditService = auditService;
            _auditRepository = auditRepository;
        }

        /// <summary>
        /// Obtiene el historial de auditoría de un compromiso específico
        /// </summary>
        /// <param name="commitmentId">ID del compromiso</param>
        /// <returns>Historial de auditoría del compromiso</returns>
        [HttpGet("commitment/{commitmentId}")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetCommitmentAuditHistory(Guid commitmentId)
        {
            try
            {
                if (commitmentId == Guid.Empty)
                {
                    return BadRequest("ID de compromiso inválido");
                }

                var auditLogs = await _auditService.GetCommitmentAuditHistoryAsync(commitmentId);
                var auditDtos = auditLogs.Select(log => new AuditLogDto
                {
                    Id = log.Id.ToString(),
                    EntityType = log.EntityType,
                    EntityId = log.EntityId.ToString(),
                    Action = log.Action,
                    UserId = log.UserId,
                    UserName = log.UserName,
                    Timestamp = log.Timestamp,
                    Details = log.Details,
                    OldValues = log.OldValues,
                    NewValues = log.NewValues,
                    IpAddress = log.IpAddress,
                    UserAgent = log.UserAgent,
                    Source = log.Source
                });

                return Ok(auditDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error obteniendo historial de auditoría: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene estadísticas de auditoría
        /// </summary>
        /// <returns>Estadísticas de auditoría del sistema</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(SponsorshipManagement.Application.Dtos.AuditStatistics), StatusCodes.Status200OK)]
        public async Task<ActionResult<SponsorshipManagement.Application.Dtos.AuditStatistics>> GetAuditStatistics()
        {
            try
            {
                var statistics = await _auditService.GetAuditStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error obteniendo estadísticas de auditoría: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene registros de auditoría con filtros y paginación
        /// </summary>
        /// <param name="request">Parámetros de consulta</param>
        /// <returns>Registros de auditoría paginados</returns>
        [HttpPost("query")]
        [ProducesResponseType(typeof(AuditPagedResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuditPagedResponse>> QueryAuditLogs([FromBody] AuditQueryRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Parámetros de consulta requeridos");
                }

                // CORRECCION: Usar el repositorio directamente para obtener logs paginados
                var (logs, totalCount) = await _auditRepository.GetPagedAsync(request.Page, request.PageSize);
                
                // Aplicar filtros básicos si están presentes
                var filteredLogs = logs.AsEnumerable();
                
                if (!string.IsNullOrEmpty(request.EntityType))
                {
                    filteredLogs = filteredLogs.Where(l => l.EntityType.Equals(request.EntityType, StringComparison.OrdinalIgnoreCase));
                }
                
                if (!string.IsNullOrEmpty(request.Action))
                {
                    filteredLogs = filteredLogs.Where(l => l.Action.Equals(request.Action, StringComparison.OrdinalIgnoreCase));
                }
                
                if (!string.IsNullOrEmpty(request.UserId))
                {
                    filteredLogs = filteredLogs.Where(l => l.UserId.Equals(request.UserId, StringComparison.OrdinalIgnoreCase));
                }
                
                if (request.StartDate.HasValue)
                {
                    filteredLogs = filteredLogs.Where(l => l.Timestamp >= request.StartDate.Value);
                }
                
                if (request.EndDate.HasValue)
                {
                    filteredLogs = filteredLogs.Where(l => l.Timestamp <= request.EndDate.Value);
                }

                var auditDtos = filteredLogs.Select(log => new AuditLogDto
                {
                    Id = log.Id.ToString(),
                    EntityType = log.EntityType,
                    EntityId = log.EntityId.ToString(),
                    Action = log.Action,
                    UserId = log.UserId,
                    UserName = log.UserName,
                    Timestamp = log.Timestamp,
                    Details = log.Details,
                    OldValues = log.OldValues,
                    NewValues = log.NewValues,
                    IpAddress = log.IpAddress,
                    UserAgent = log.UserAgent,
                    Source = log.Source
                });

                var finalCount = auditDtos.Count();
                var totalPages = (int)Math.Ceiling((double)finalCount / request.PageSize);

                var response = new AuditPagedResponse
                {
                    Logs = auditDtos,
                    TotalCount = finalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = request.Page < totalPages,
                    HasPreviousPage = request.Page > 1
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error consultando registros de auditoría: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todos los registros de auditoría (sin filtros)
        /// </summary>
        /// <returns>Todos los registros de auditoría</returns>
        [HttpGet("all")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAllAuditLogs()
        {
            try
            {
                var auditLogs = await _auditRepository.GetAllAsync();
                var auditDtos = auditLogs.Select(log => new AuditLogDto
                {
                    Id = log.Id.ToString(),
                    EntityType = log.EntityType,
                    EntityId = log.EntityId.ToString(),
                    Action = log.Action,
                    UserId = log.UserId,
                    UserName = log.UserName,
                    Timestamp = log.Timestamp,
                    Details = log.Details,
                    OldValues = log.OldValues,
                    NewValues = log.NewValues,
                    IpAddress = log.IpAddress,
                    UserAgent = log.UserAgent,
                    Source = log.Source
                });

                return Ok(auditDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error obteniendo registros de auditoría: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene registros de auditoría por acción específica
        /// </summary>
        /// <param name="action">Tipo de acción (CREATE, UPDATE, DELETE, etc.)</param>
        /// <returns>Registros de auditoría filtrados por acción</returns>
        [HttpGet("action/{action}")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByAction(string action)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(action))
                {
                    return BadRequest("La acción es requerida");
                }

                var auditLogs = await _auditRepository.GetByActionAsync(action.ToUpper());
                var auditDtos = auditLogs.Select(log => new AuditLogDto
                {
                    Id = log.Id.ToString(),
                    EntityType = log.EntityType,
                    EntityId = log.EntityId.ToString(),
                    Action = log.Action,
                    UserId = log.UserId,
                    UserName = log.UserName,
                    Timestamp = log.Timestamp,
                    Details = log.Details,
                    OldValues = log.OldValues,
                    NewValues = log.NewValues,
                    IpAddress = log.IpAddress,
                    UserAgent = log.UserAgent,
                    Source = log.Source
                });

                return Ok(auditDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error obteniendo registros por acción: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene registros de auditoría por rango de fechas
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Registros de auditoría en el rango de fechas</returns>
        [HttpGet("date-range")]
        [ProducesResponseType(typeof(IEnumerable<AuditLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate == default || endDate == default)
                {
                    return BadRequest("Las fechas de inicio y fin son requeridas");
                }

                if (startDate > endDate)
                {
                    return BadRequest("La fecha de inicio no puede ser mayor que la fecha de fin");
                }

                var auditLogs = await _auditRepository.GetByDateRangeAsync(startDate, endDate);
                var auditDtos = auditLogs.Select(log => new AuditLogDto
                {
                    Id = log.Id.ToString(),
                    EntityType = log.EntityType,
                    EntityId = log.EntityId.ToString(),
                    Action = log.Action,
                    UserId = log.UserId,
                    UserName = log.UserName,
                    Timestamp = log.Timestamp,
                    Details = log.Details,
                    OldValues = log.OldValues,
                    NewValues = log.NewValues,
                    IpAddress = log.IpAddress,
                    UserAgent = log.UserAgent,
                    Source = log.Source
                });

                return Ok(auditDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error obteniendo registros por fecha: {ex.Message}");
            }
        }

        /// <summary>
        /// Limpia registros de auditoría antiguos
        /// </summary>
        /// <param name="daysToKeep">Días de registros a mantener (por defecto 90)</param>
        /// <returns>Número de registros eliminados</returns>
        [HttpDelete("cleanup")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CleanupOldLogs([FromQuery] int daysToKeep = 90)
        {
            try
            {
                if (daysToKeep <= 0)
                {
                    return BadRequest("El número de días debe ser mayor a 0");
                }

                var deletedCount = await _auditService.CleanupOldAuditLogsAsync(daysToKeep);
                
                return Ok(new
                {
                    message = "Limpieza de auditoría completada",
                    deletedCount = deletedCount,
                    daysToKeep = daysToKeep,
                    cleanupDate = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error limpiando registros de auditoría: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene tipos de acciones auditadas disponibles
        /// </summary>
        /// <returns>Lista de tipos de acciones</returns>
        [HttpGet("actions")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        public ActionResult<string[]> GetAvailableActions()
        {
            var actions = new[]
            {
                AuditActions.CREATE,
                AuditActions.UPDATE,
                AuditActions.DELETE,
                AuditActions.STATUS_CHANGE,
                AuditActions.VIEW,
                AuditActions.EXPORT
            };

            return Ok(actions);
        }

        /// <summary>
        /// Obtiene tipos de entidades auditadas disponibles
        /// </summary>
        /// <returns>Lista de tipos de entidades</returns>
        [HttpGet("entity-types")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        public ActionResult<string[]> GetAvailableEntityTypes()
        {
            var entityTypes = new[]
            {
                AuditEntityTypes.COMMITMENT,
                AuditEntityTypes.CONTRACT,
                AuditEntityTypes.SPONSOR,
                AuditEntityTypes.EVENT
            };

            return Ok(entityTypes);
        }
    }
}
