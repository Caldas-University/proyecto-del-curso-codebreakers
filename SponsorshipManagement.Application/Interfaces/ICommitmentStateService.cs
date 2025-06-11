using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface ICommitmentStateService
    {
        /// <summary>
        /// 🎯 CU-PA-02.01.4: Asigna estado inicial "Pendiente"
        /// </summary>
        Task<bool> InitializeCommitmentStateAsync(Guid commitmentId);
        
        /// <summary>
        /// Cambia el estado de un compromiso
        /// </summary>
        Task<bool> ChangeCommitmentStatusAsync(Guid commitmentId, CommitmentStatus newStatus, string reason = "");
        
        /// <summary>
        /// Procesa compromisos vencidos automáticamente
        /// </summary>
        Task<int> ProcessOverdueCommitmentsAsync();
        
        /// <summary>
        /// Obtiene estadísticas de estados
        /// </summary>
        Task<StateStatistics> GetStateStatisticsAsync();
        
        /// <summary>
        /// Valida si una transición de estado es válida
        /// </summary>
        Task<bool> ValidateStateTransitionAsync(Guid commitmentId, CommitmentStatus newStatus);
    }

    /// <summary>
    /// Estadísticas de estados de compromisos
    /// </summary>
    public class StateStatistics
    {
        public int TotalCommitments { get; set; }
        public int PendingCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int OverdueCount { get; set; }
        public int OnHoldCount { get; set; }
        
        public double CompletionRate { get; set; }
        public double PendingRate { get; set; }
        public double OverdueRate { get; set; }
    }
}
