using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface IContractRepository
    {
        Task<Contract?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> ExistsAndIsValidForCommitmentsAsync(Guid id);
        Task<IEnumerable<Contract>> GetAllAsync();
        Task<IEnumerable<Contract>> GetActiveContractsAsync();
        Task<bool> AddAsync(Contract contract);
        Task<bool> UpdateAsync(Contract contract);
        Task<bool> DeleteAsync(Guid id);
    }
}
