using System.Collections.Generic;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface IAdvertisingBenefitRepository
    {
        Task<IEnumerable<AdvertisingBenefit>> GetAllAsync();
        Task<AdvertisingBenefit?> GetByIdAsync(Guid id);
        Task<IEnumerable<AdvertisingBenefit>> GetByTypeAsync(string benefitType);
        Task<AdvertisingBenefit> CreateAsync(AdvertisingBenefit benefit);
        Task<AdvertisingBenefit> UpdateAsync(AdvertisingBenefit benefit);
        Task<bool> DeleteAsync(Guid id);
    }
}