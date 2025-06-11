using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IAdvertisingBenefitService
    {
        Task<IEnumerable<AdvertisingBenefitDto>> GetAllBenefitsAsync();
        Task<AdvertisingBenefitDto?> GetBenefitByIdAsync(Guid id);
        Task<IEnumerable<AdvertisingBenefitDto>> GetBenefitsByTypeAsync(string benefitType);
        Task<AdvertisingBenefitDto> CreateBenefitAsync(AdvertisingBenefitDto benefitDto);
        Task<AdvertisingBenefitDto> UpdateBenefitAsync(AdvertisingBenefitDto benefitDto);
        Task<bool> DeleteBenefitAsync(Guid id);
        Task<bool> BenefitExistsAsync(Guid id);
    }
}