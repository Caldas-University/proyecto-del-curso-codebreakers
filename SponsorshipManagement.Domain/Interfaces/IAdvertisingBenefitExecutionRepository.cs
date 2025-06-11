using System.Collections.Generic;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface IAdvertisingBenefitExecutionRepository
    {
        Task<IEnumerable<AdvertisingBenefitExecution>> GetAllAsync();
        Task<AdvertisingBenefitExecution?> GetByIdAsync(Guid id);
        Task<IEnumerable<AdvertisingBenefitExecution>> GetBySponsorAsync(string sponsorDocumentNumber);
        Task<IEnumerable<AdvertisingBenefitExecution>> GetByEventAsync(string eventId);
        Task<IEnumerable<AdvertisingBenefitExecution>> GetByContractAsync(Guid contractId);
        Task<IEnumerable<AdvertisingBenefitExecution>> GetByStatusAsync(string status);
        Task<AdvertisingBenefitExecution> CreateAsync(AdvertisingBenefitExecution execution);
        Task<AdvertisingBenefitExecution> UpdateAsync(AdvertisingBenefitExecution execution);
        Task<bool> DeleteAsync(Guid id);
    }
}