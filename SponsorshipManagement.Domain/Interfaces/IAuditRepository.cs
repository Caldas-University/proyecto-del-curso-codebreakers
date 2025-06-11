using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    /// <summary>
    /// Repositorio para gestión de registros de auditoría
    /// CU-PA-02.01.5: Registro de auditoría al crear compromiso
    /// </summary>
    public interface IAuditRepository
    {
        /// <summary>
        /// Registra un evento de auditoría
        /// </summary>
        Task<bool> LogAsync(AuditLog auditLog);
        
        /// <summary>
        /// Obtiene registros de auditoría por entidad
        /// </summary>
        Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId);
        
        /// <summary>
        /// Obtiene registros de auditoría por usuario
        /// </summary>
        Task<IEnumerable<AuditLog>> GetByUserAsync(string userId);
        
        /// <summary>
        /// Obtiene registros de auditoría por fecha
        /// </summary>
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        
        /// <summary>
        /// Obtiene registros de auditoría por acción
        /// </summary>
        Task<IEnumerable<AuditLog>> GetByActionAsync(string action);
        
        /// <summary>
        /// Obtiene todos los registros de auditoría
        /// </summary>
        Task<IEnumerable<AuditLog>> GetAllAsync();
        
        /// <summary>
        /// Obtiene registros de auditoría paginados
        /// </summary>
        Task<(IEnumerable<AuditLog> Logs, int TotalCount)> GetPagedAsync(int page, int pageSize);
        
        /// <summary>
        /// Limpia registros de auditoría antiguos
        /// </summary>
        Task<int> CleanupOldLogsAsync(DateTime cutoffDate);
    }
}
