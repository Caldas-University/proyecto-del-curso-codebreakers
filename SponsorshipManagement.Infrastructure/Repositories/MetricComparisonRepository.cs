using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class MetricComparisonRepository : IMetricComparisonRepository
    {
        private readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/metric_comparisons.json";

        private async Task<List<MetricComparison>> LoadComparisonsAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return new List<MetricComparison>();

            using FileStream fs = File.OpenRead(_jsonFilePath);
            var comparisons = await JsonSerializer.DeserializeAsync<List<MetricComparison>>(fs);
            return comparisons ?? new List<MetricComparison>();
        }

        private async Task SaveComparisonsAsync(List<MetricComparison> comparisons)
        {
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            using FileStream fs = File.Create(_jsonFilePath);
            await JsonSerializer.SerializeAsync(fs, comparisons, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task<IEnumerable<MetricComparison>> GetAllAsync()
        {
            return await LoadComparisonsAsync();
        }

        public async Task<MetricComparison?> GetByIdAsync(Guid id)
        {
            var comparisons = await LoadComparisonsAsync();
            return comparisons.FirstOrDefault(c => c.Id == id);
        }

        public async Task<IEnumerable<MetricComparison>> GetByTypeAsync(string comparisonType)
        {
            var comparisons = await LoadComparisonsAsync();
            return comparisons.Where(c => c.ComparisonType.Equals(comparisonType, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<MetricComparison> CreateAsync(MetricComparison comparison)
        {
            var comparisons = await LoadComparisonsAsync();
            comparison.Id = Guid.NewGuid();
            comparison.CreatedDate = DateTime.UtcNow;
            comparisons.Add(comparison);
            await SaveComparisonsAsync(comparisons);
            return comparison;
        }

        public async Task<MetricComparison> UpdateAsync(MetricComparison comparison)
        {
            var comparisons = await LoadComparisonsAsync();
            var index = comparisons.FindIndex(c => c.Id == comparison.Id);
            if (index >= 0)
            {
                comparisons[index] = comparison;
                await SaveComparisonsAsync(comparisons);
            }
            return comparison;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var comparisons = await LoadComparisonsAsync();
            var comparison = comparisons.FirstOrDefault(c => c.Id == id);
            if (comparison != null)
            {
                comparisons.Remove(comparison);
                await SaveComparisonsAsync(comparisons);
                return true;
            }
            return false;
        }
    }
}