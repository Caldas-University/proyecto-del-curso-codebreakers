namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para estadísticas de auditoría
    /// CU-PA-02.01.5: Métricas de auditoría del sistema
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