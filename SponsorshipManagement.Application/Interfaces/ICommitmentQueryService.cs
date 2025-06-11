using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.Application.Interfaces
{
    /// <summary>
    /// Servicio para consulta avanzada de compromisos
    /// CU-PA-02.04.1: Servicio para consultar compromisos
    /// </summary>
    public interface ICommitmentQueryService
    {
        /// <summary>
        /// Obtiene compromisos con filtros avanzados
        /// </summary>
        /// <param name="filters">Filtros de consulta</param>
        /// <returns>Lista de compromisos filtrados y enriquecidos</returns>
        Task<IEnumerable<CommitmentQueryDto>> GetCommitmentsWithFiltersAsync(CommitmentQueryFilters filters);

        /// <summary>
        /// Obtiene todos los compromisos de un contrato con información enriquecida
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Compromisos con datos enriquecidos</returns>
        Task<IEnumerable<CommitmentQueryDto>> GetEnrichedCommitmentsByContractAsync(Guid contractId);

        /// <summary>
        /// Obtiene compromisos agrupados por estado
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Compromisos agrupados por estado</returns>
        Task<Dictionary<string, List<CommitmentQueryDto>>> GetCommitmentsGroupedByStatusAsync(Guid contractId);

        /// <summary>
        /// Obtiene compromisos críticos (vencidos o próximos a vencer)
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Compromisos críticos</returns>
        Task<IEnumerable<CommitmentQueryDto>> GetCriticalCommitmentsAsync(Guid contractId);

        /// <summary>
        /// Busca compromisos por texto en descripción u obligaciones
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <param name="searchText">Texto a buscar</param>
        /// <returns>Compromisos que coinciden con la búsqueda</returns>
        Task<IEnumerable<CommitmentQueryDto>> SearchCommitmentsAsync(Guid contractId, string searchText);

        /// <summary>
        /// Obtiene el historial de cambios de estado de compromisos de un contrato
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Historial de cambios</returns>
        Task<IEnumerable<CommitmentStatusHistoryDto>> GetStatusHistoryAsync(Guid contractId);
    }

    /// <summary>
    /// DTO para historial de cambios de estado
    /// </summary>
    public class CommitmentStatusHistoryDto
    {
        public string CommitmentId { get; set; } = string.Empty;
        public string CommitmentDescription { get; set; } = string.Empty;
        public string FromStatus { get; set; } = string.Empty;
        public string ToStatus { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}
