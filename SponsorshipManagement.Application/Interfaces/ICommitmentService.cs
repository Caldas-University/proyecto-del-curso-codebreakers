using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface ICommitmentService
    {
        Task<CommitmentDto?> GetCommitmentByIdAsync(Guid id);
        Task<IEnumerable<CommitmentDto>> GetCommitmentsByContractIdAsync(Guid contractId);
        Task<IEnumerable<CommitmentDto>> GetAllCommitmentsAsync();
        Task<CommitmentDto?> CreateCommitmentAsync(CreateCommitmentRequest request);
        Task<CommitmentDto?> UpdateCommitmentAsync(Guid id, CreateCommitmentRequest request);
        Task<bool> DeleteCommitmentAsync(Guid id);
        Task<IEnumerable<CommitmentDto>> GetOverdueCommitmentsAsync();
    }
}
