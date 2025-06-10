using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface ICommitmentService
    {
        /// <summary>
        /// Obtiene un compromiso por su ID
        /// </summary>
        Task<CommitmentDto?> GetCommitmentByIdAsync(Guid id);
        
        /// <summary>
        /// Obtiene todos los compromisos
        /// </summary>
        Task<IEnumerable<CommitmentDto>> GetAllCommitmentsAsync();
        
        /// <summary>
        /// Obtiene compromisos por ID de contrato
        /// </summary>
        Task<IEnumerable<CommitmentDto>> GetCommitmentsByContractIdAsync(Guid contractId);
        
        /// <summary>
        /// Crea un nuevo compromiso
        /// </summary>
        Task<CommitmentDto?> CreateCommitmentAsync(CreateCommitmentRequest request);
        
        /// <summary>
        /// Actualiza un compromiso existente
        /// </summary>
        Task<CommitmentDto?> UpdateCommitmentAsync(Guid id, CreateCommitmentRequest request);
        
        /// <summary>
        /// Elimina un compromiso
        /// </summary>
        Task<bool> DeleteCommitmentAsync(Guid id);
        
        /// <summary>
        /// Obtiene compromisos vencidos
        /// </summary>
        Task<IEnumerable<CommitmentDto>> GetOverdueCommitmentsAsync();
        
        /// <summary>
        /// 🎯 CU-PA-02.01.4: Obtiene estadísticas de compromisos por contrato
        /// </summary>
        Task<CommitmentStatisticsDto> GetCommitmentStatisticsAsync(Guid contractId);
    }
}