using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface ICommitmentRepository
    {
        Task<Commitment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Commitment>> GetByContractIdAsync(Guid contractId);
        Task<IEnumerable<Commitment>> GetByStatusAsync(CommitmentStatus status);
        Task<IEnumerable<Commitment>> GetAllAsync();
        Task<bool> AddAsync(Commitment commitment);
        Task<bool> UpdateAsync(Commitment commitment);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ContractExistsAsync(Guid contractId);
        Task<IEnumerable<Commitment>> GetOverdueCommitmentsAsync();
    }
}
