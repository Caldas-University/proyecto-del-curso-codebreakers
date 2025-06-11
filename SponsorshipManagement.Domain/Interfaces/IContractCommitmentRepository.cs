using System.Collections.Generic;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface IContractCommitmentRepository
    {
        Task<IEnumerable<ContractCommitment>> GetAllAsync();
        Task<ContractCommitment?> GetByIdAsync(Guid id);
        Task<IEnumerable<ContractCommitment>> GetByContractIdAsync(Guid contractId);
        Task<IEnumerable<ContractCommitment>> GetByStatusAsync(CommitmentStatus status);
        Task<IEnumerable<ContractCommitment>> GetOverdueCommitmentsAsync();
        Task<ContractCommitment> CreateAsync(ContractCommitment commitment);
        Task<ContractCommitment> UpdateAsync(ContractCommitment commitment);
        Task<bool> DeleteAsync(Guid id);
    }
}