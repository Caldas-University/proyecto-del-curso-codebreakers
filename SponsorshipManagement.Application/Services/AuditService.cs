using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using SponsorshipManagement.Application.Interfaces;
using System.Text.Json;

namespace SponsorshipManagement.Application.Services
{
    /// <summary>
    /// Servicio para gestión de auditoría
    /// CU-PA-02.01.5: Registro de auditoría al crear compromiso
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;

        public AuditService(IAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        /// <summary>
        /// CU-PA-02.01.5: Registra la creación de un compromiso
        /// </summary>
        public async Task<bool> LogCommitmentCreationAsync(Commitment commitment, string userId = "system", string userName = "Sistema")
        {
            try
            {
                Console.WriteLine($"🔍 AuditService.LogCommitmentCreationAsync llamado para compromiso {commitment.Id}");
                
                var auditLog = new AuditLog(
                    AuditEntityTypes.COMMITMENT,
                    commitment.Id,
                    AuditActions.CREATE,
                    userId,
                    userName,
                    $"Compromiso creado: {commitment.Description}"
                );

                // Serializar los valores nuevos
                var newValues = new
                {
                    commitment.Id,
                    commitment.ContractId,
                    commitment.Description,
                    commitment.Obligations,
                    commitment.DueDate,
                    commitment.Responsible,
                    commitment.Status,
                    commitment.CreatedAt
                };

                auditLog.NewValues = JsonSerializer.Serialize(newValues);
                auditLog.Source = "CU-PA-02.01.5";

                Console.WriteLine($"🔍 Llamando a _auditRepository.LogAsync...");
                var success = await _auditRepository.LogAsync(auditLog);
                
                Console.WriteLine($"🔍 Resultado del repositorio: {success}");
                
                if (success)
                {
                    Console.WriteLine($"✅ CU-PA-02.01.5: Auditoría registrada para compromiso {commitment.Id}");
                }
                else
                {
                    Console.WriteLine($"❌ Error registrando auditoría para compromiso {commitment.Id}");
                }
                
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en AuditService.LogCommitmentCreationAsync: {ex.Message}");
                Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// Registra la actualización de un compromiso
        /// </summary>
        public async Task<bool> LogCommitmentUpdateAsync(Commitment oldCommitment, Commitment newCommitment, string userId = "system", string userName = "Sistema")
        {
            try
            {
                var auditLog = new AuditLog(
                    AuditEntityTypes.COMMITMENT,
                    newCommitment.Id,
                    AuditActions.UPDATE,
                    userId,
                    userName,
                    $"Compromiso actualizado: {newCommitment.Description}"
                );

                var oldValues = new { oldCommitment.Description, oldCommitment.Obligations, oldCommitment.Status };
                var newValues = new { newCommitment.Description, newCommitment.Obligations, newCommitment.Status };

                auditLog.OldValues = JsonSerializer.Serialize(oldValues);
                auditLog.NewValues = JsonSerializer.Serialize(newValues);

                return await _auditRepository.LogAsync(auditLog);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registrando auditoría de actualización: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Registra cambio de estado de un compromiso
        /// </summary>
        public async Task<bool> LogCommitmentStatusChangeAsync(Guid commitmentId, CommitmentStatus oldStatus, CommitmentStatus newStatus, string reason, string userId = "system", string userName = "Sistema")
        {
            try
            {
                var auditLog = new AuditLog(
                    AuditEntityTypes.COMMITMENT,
                    commitmentId,
                    AuditActions.STATUS_CHANGE,
                    userId,
                    userName,
                    $"Estado cambiado de {oldStatus} a {newStatus}. Razón: {reason}"
                );

                var changeDetails = new
                {
                    OldStatus = oldStatus.ToString(),
                    NewStatus = newStatus.ToString(),
                    Reason = reason,
                    ChangeTimestamp = DateTime.UtcNow
                };

                auditLog.NewValues = JsonSerializer.Serialize(changeDetails);
                return await _auditRepository.LogAsync(auditLog);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registrando auditoría de cambio de estado: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Sobrecarga para CommitmentStateService (sin return bool)
        /// </summary>
        public async Task LogCommitmentStatusChangeAsync(Guid commitmentId, CommitmentStatus fromStatus, CommitmentStatus toStatus, string reason)
        {
            await LogCommitmentStatusChangeAsync(commitmentId, fromStatus, toStatus, reason, "system", "Sistema");
        }

        /// <summary>
        /// Registra la eliminación de un compromiso
        /// </summary>
        public async Task<bool> LogCommitmentDeletionAsync(Commitment commitment, string userId = "system", string userName = "Sistema")
        {
            try
            {
                var auditLog = new AuditLog(
                    AuditEntityTypes.COMMITMENT,
                    commitment.Id,
                    AuditActions.DELETE,
                    userId,
                    userName,
                    $"Compromiso eliminado: {commitment.Description}"
                );

                var deletedValues = new
                {
                    commitment.Id,
                    commitment.Description,
                    commitment.Status
                };

                auditLog.OldValues = JsonSerializer.Serialize(deletedValues);
                return await _auditRepository.LogAsync(auditLog);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registrando auditoría de eliminación: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtiene el historial de auditoría de un compromiso
        /// </summary>
        public async Task<IEnumerable<AuditLog>> GetCommitmentAuditHistoryAsync(Guid commitmentId)
        {
            try
            {
                return await _auditRepository.GetByEntityAsync(AuditEntityTypes.COMMITMENT, commitmentId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo historial de auditoría: {ex.Message}");
                return Enumerable.Empty<AuditLog>();
            }
        }

        /// <summary>
        /// Obtiene estadísticas de auditoría
        /// </summary>
        public async Task<AuditStatistics> GetAuditStatisticsAsync()
        {
            try
            {
                var allLogs = await _auditRepository.GetAllAsync();
                var logsList = allLogs.ToList();

                return new AuditStatistics
                {
                    TotalLogs = logsList.Count,
                    CreateActions = logsList.Count(l => l.Action == AuditActions.CREATE),
                    UpdateActions = logsList.Count(l => l.Action == AuditActions.UPDATE),
                    DeleteActions = logsList.Count(l => l.Action == AuditActions.DELETE),
                    StatusChangeActions = logsList.Count(l => l.Action == AuditActions.STATUS_CHANGE),
                    CommitmentLogs = logsList.Count(l => l.EntityType == AuditEntityTypes.COMMITMENT),
                    LastLogTimestamp = logsList.Any() ? logsList.Max(l => l.Timestamp) : null,
                    FirstLogTimestamp = logsList.Any() ? logsList.Min(l => l.Timestamp) : null
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo estadísticas de auditoría: {ex.Message}");
                return new AuditStatistics();
            }
        }

        /// <summary>
        /// Limpia registros de auditoría antiguos
        /// </summary>
        public async Task<int> CleanupOldAuditLogsAsync(int daysToKeep = 90)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
                return await _auditRepository.CleanupOldLogsAsync(cutoffDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error limpiando logs antiguos: {ex.Message}");
                return 0;
            }
        }
    }

    /// <summary>
    /// Estadísticas de auditoría
    /// </summary>
    public class AuditStatistics
    {
        public int TotalLogs { get; set; }
        public int CreateActions { get; set; }
        public int UpdateActions { get; set; }
        public int DeleteActions { get; set; }
        public int StatusChangeActions { get; set; }
        public int CommitmentLogs { get; set; }
        public DateTime? FirstLogTimestamp { get; set; }
        public DateTime? LastLogTimestamp { get; set; }
    }
}