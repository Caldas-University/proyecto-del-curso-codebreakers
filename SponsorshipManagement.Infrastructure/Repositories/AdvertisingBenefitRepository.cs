using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class AdvertisingBenefitRepository : IAdvertisingBenefitRepository
    {
        private readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/advertising_benefits.json";

        private async Task<List<AdvertisingBenefit>> LoadBenefitsAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return new List<AdvertisingBenefit>();

            using FileStream fs = File.OpenRead(_jsonFilePath);
            var benefits = await JsonSerializer.DeserializeAsync<List<AdvertisingBenefit>>(fs);
            return benefits ?? new List<AdvertisingBenefit>();
        }

        private async Task SaveBenefitsAsync(List<AdvertisingBenefit> benefits)
        {
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            using FileStream fs = File.Create(_jsonFilePath);
            await JsonSerializer.SerializeAsync(fs, benefits, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task<IEnumerable<AdvertisingBenefit>> GetAllAsync()
        {
            return await LoadBenefitsAsync();
        }

        public async Task<AdvertisingBenefit?> GetByIdAsync(Guid id)
        {
            var benefits = await LoadBenefitsAsync();
            return benefits.FirstOrDefault(b => b.Id == id);
        }

        public async Task<IEnumerable<AdvertisingBenefit>> GetByTypeAsync(string benefitType)
        {
            var benefits = await LoadBenefitsAsync();
            return benefits.Where(b => b.BenefitType.Equals(benefitType, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<AdvertisingBenefit> CreateAsync(AdvertisingBenefit benefit)
        {
            var benefits = await LoadBenefitsAsync();
            benefit.Id = Guid.NewGuid();
            benefit.CreatedDate = DateTime.UtcNow;
            benefits.Add(benefit);
            await SaveBenefitsAsync(benefits);
            return benefit;
        }

        public async Task<AdvertisingBenefit> UpdateAsync(AdvertisingBenefit benefit)
        {
            var benefits = await LoadBenefitsAsync();
            var index = benefits.FindIndex(b => b.Id == benefit.Id);
            if (index >= 0)
            {
                benefits[index] = benefit;
                await SaveBenefitsAsync(benefits);
            }
            return benefit;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var benefits = await LoadBenefitsAsync();
            var benefit = benefits.FirstOrDefault(b => b.Id == id);
            if (benefit != null)
            {
                benefits.Remove(benefit);
                await SaveBenefitsAsync(benefits);
                return true;
            }
            return false;
        }
    }
}