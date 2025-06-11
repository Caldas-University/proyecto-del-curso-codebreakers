namespace SponsorshipManagement.Domain.Entities
{
    /// <summary>
    /// Entidad para registro de auditoría
    /// CU-PA-02.01.5: Registro de auditoría al crear compromiso
    /// </summary>
    public class AuditLog
    {
        public Guid Id { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Details { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public AuditLog()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Constructor para crear registro de auditoría
        /// </summary>
        public AuditLog(string entityType, Guid entityId, string action, string userId, string userName, string details)
            : this()
        {
            EntityType = entityType;
            EntityId = entityId;
            Action = action;
            UserId = userId;
            UserName = userName;
            Details = details;
            Source = "CU-PA-02.01.5";
        }
    }

    /// <summary>
    /// Tipos de acciones auditadas
    /// </summary>
    public static class AuditActions
    {
        public const string CREATE = "CREATE";
        public const string UPDATE = "UPDATE";
        public const string DELETE = "DELETE";
        public const string STATUS_CHANGE = "STATUS_CHANGE";
        public const string VIEW = "VIEW";
        public const string EXPORT = "EXPORT";
    }

    /// <summary>
    /// Tipos de entidades auditadas
    /// </summary>
    public static class AuditEntityTypes
    {
        public const string COMMITMENT = "Commitment";
        public const string CONTRACT = "Contract";
        public const string SPONSOR = "Sponsor";
        public const string EVENT = "Event";
    }
}
