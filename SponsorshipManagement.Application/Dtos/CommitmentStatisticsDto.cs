namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para estadísticas de compromisos por contrato
    /// 🎯 CU-PA-02.01.4: Métricas de gestión de estados
    /// </summary>
    public class CommitmentStatisticsDto
    {
        /// <summary>
        /// ID del contrato
        /// </summary>
        public string ContractId { get; set; } = string.Empty;
        
        /// <summary>
        /// Total de compromisos
        /// </summary>
        public int TotalCommitments { get; set; }
        
        /// <summary>
        /// Compromisos pendientes
        /// </summary>
        public int PendingCommitments { get; set; }
        
        /// <summary>
        /// Compromisos en progreso
        /// </summary>
        public int InProgressCommitments { get; set; }
        
        /// <summary>
        /// Compromisos completados
        /// </summary>
        public int CompletedCommitments { get; set; }
        
        /// <summary>
        /// Compromisos cancelados
        /// </summary>
        public int CancelledCommitments { get; set; }
        
        /// <summary>
        /// Compromisos vencidos
        /// </summary>
        public int OverdueCommitments { get; set; }
        
        /// <summary>
        /// Compromisos en espera
        /// </summary>
        public int OnHoldCommitments { get; set; }
        
        /// <summary>
        /// Tasa de completación (porcentaje)
        /// </summary>
        public double CompletionRate { get; set; }
        
        /// <summary>
        /// Tasa de compromisos pendientes (porcentaje)
        /// </summary>
        public double PendingRate { get; set; }
        
        /// <summary>
        /// Tasa de compromisos vencidos (porcentaje)
        /// </summary>
        public double OverdueRate { get; set; }
        
        /// <summary>
        /// Fecha de generación de las estadísticas
        /// </summary>
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Título del contrato (opcional)
        /// </summary>
        public string? ContractTitle { get; set; }
        
        /// <summary>
        /// Estado del contrato (opcional)
        /// </summary>
        public string? ContractStatus { get; set; }
    }
}