using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class AdvertisingBenefitService : IAdvertisingBenefitService
    {
        private readonly IAdvertisingBenefitRepository _benefitRepository;

        public AdvertisingBenefitService(IAdvertisingBenefitRepository benefitRepository)
        {
            _benefitRepository = benefitRepository;
        }

        public async Task<IEnumerable<AdvertisingBenefitDto>> GetAllBenefitsAsync()
        {
            var benefits = await _benefitRepository.GetAllAsync();
            return benefits.Select(b => new AdvertisingBenefitDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                BenefitType = b.BenefitType,
                EstimatedValue = b.EstimatedValue,
                IsActive = b.IsActive,
                CreatedDate = b.CreatedDate
            });
        }

        public async Task<AdvertisingBenefitDto?> GetBenefitByIdAsync(Guid id)
        {
            var benefit = await _benefitRepository.GetByIdAsync(id);
            if (benefit == null) return null;

            return new AdvertisingBenefitDto
            {
                Id = benefit.Id,
                Name = benefit.Name,
                Description = benefit.Description,
                BenefitType = benefit.BenefitType,
                EstimatedValue = benefit.EstimatedValue,
                IsActive = benefit.IsActive,
                CreatedDate = benefit.CreatedDate
            };
        }

        public async Task<IEnumerable<AdvertisingBenefitDto>> GetBenefitsByTypeAsync(string benefitType)
        {
            var benefits = await _benefitRepository.GetByTypeAsync(benefitType);
            return benefits.Select(b => new AdvertisingBenefitDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                BenefitType = b.BenefitType,
                EstimatedValue = b.EstimatedValue,
                IsActive = b.IsActive,
                CreatedDate = b.CreatedDate
            });
        }

        public async Task<AdvertisingBenefitDto> CreateBenefitAsync(AdvertisingBenefitDto benefitDto)
        {
            var benefit = new AdvertisingBenefit
            {
                Name = benefitDto.Name,
                Description = benefitDto.Description,
                BenefitType = benefitDto.BenefitType,
                EstimatedValue = benefitDto.EstimatedValue,
                IsActive = benefitDto.IsActive
            };

            var createdBenefit = await _benefitRepository.CreateAsync(benefit);
            
            return new AdvertisingBenefitDto
            {
                Id = createdBenefit.Id,
                Name = createdBenefit.Name,
                Description = createdBenefit.Description,
                BenefitType = createdBenefit.BenefitType,
                EstimatedValue = createdBenefit.EstimatedValue,
                IsActive = createdBenefit.IsActive,
                CreatedDate = createdBenefit.CreatedDate
            };
        }

        public async Task<AdvertisingBenefitDto> UpdateBenefitAsync(AdvertisingBenefitDto benefitDto)
        {
            var benefit = new AdvertisingBenefit
            {
                Id = benefitDto.Id,
                Name = benefitDto.Name,
                Description = benefitDto.Description,
                BenefitType = benefitDto.BenefitType,
                EstimatedValue = benefitDto.EstimatedValue,
                IsActive = benefitDto.IsActive,
                CreatedDate = benefitDto.CreatedDate
            };

            var updatedBenefit = await _benefitRepository.UpdateAsync(benefit);
            
            return new AdvertisingBenefitDto
            {
                Id = updatedBenefit.Id,
                Name = updatedBenefit.Name,
                Description = updatedBenefit.Description,
                BenefitType = updatedBenefit.BenefitType,
                EstimatedValue = updatedBenefit.EstimatedValue,
                IsActive = updatedBenefit.IsActive,
                CreatedDate = updatedBenefit.CreatedDate
            };
        }

        public async Task<bool> DeleteBenefitAsync(Guid id)
        {
            return await _benefitRepository.DeleteAsync(id);
        }

        public async Task<bool> BenefitExistsAsync(Guid id)
        {
            var benefit = await _benefitRepository.GetByIdAsync(id);
            return benefit != null;
        }
    }
}