using System.Collections.Generic;
using System.Threading.Tasks;
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
        Task<Contract> UpdateAsync(Contract contract);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Contract>> GetBySponsorIdAsync(string sponsorId);
        Task<IEnumerable<Contract>> GetByEventIdAsync(string eventId);
        Task<IEnumerable<Contract>> GetByStatusAsync(ContractStatus status);
        Task<Contract> CreateAsync(Contract contract);
    }

}
