using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Application.Services;

namespace SponsorshipManagement.Application.Interfaces
{
    /// <summary>
    /// Interfaz para servicio de auditoría
    /// CU-PA-02.01.5: Registro de auditoría al crear compromiso
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// CU-PA-02.01.5: Registra la creación de un compromiso
        /// </summary>
        Task<bool> LogCommitmentCreationAsync(Commitment commitment, string userId = "system", string userName = "Sistema");
        
        /// <summary>
        /// Registra la actualización de un compromiso
        /// </summary>
        Task<bool> LogCommitmentUpdateAsync(Commitment oldCommitment, Commitment newCommitment, string userId = "system", string userName = "Sistema");
        
        /// <summary>
        /// Registra cambio de estado de un compromiso
        /// </summary>
        Task<bool> LogCommitmentStatusChangeAsync(Guid commitmentId, CommitmentStatus oldStatus, CommitmentStatus newStatus, string reason, string userId = "system", string userName = "Sistema");
        
        /// <summary>
        /// Registra la eliminación de un compromiso
        /// </summary>
        Task<bool> LogCommitmentDeletionAsync(Commitment commitment, string userId = "system", string userName = "Sistema");
        
        /// <summary>
        /// Obtiene el historial de auditoría de un compromiso
        /// </summary>
        Task<IEnumerable<AuditLog>> GetCommitmentAuditHistoryAsync(Guid commitmentId);
        
        /// <summary>
        /// Obtiene estadísticas de auditoría
        /// </summary>
        Task<AuditStatistics> GetAuditStatisticsAsync();
        
        /// <summary>
        /// Limpia registros de auditoría antiguos
        /// </summary>
        Task<int> CleanupOldAuditLogsAsync(int daysToKeep = 90);
        
        // AGREGAR el método que usa CommitmentStateService
        Task LogCommitmentStatusChangeAsync(Guid commitmentId, CommitmentStatus fromStatus, CommitmentStatus toStatus, string reason);
    }
}
