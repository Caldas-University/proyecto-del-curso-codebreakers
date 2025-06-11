using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class MetricRepository : IMetricRepository
    {
        private readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/metrics.json";

        private async Task<List<Metric>> LoadMetricsAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return new List<Metric>();

            using FileStream fs = File.OpenRead(_jsonFilePath);
            var metrics = await JsonSerializer.DeserializeAsync<List<Metric>>(fs);
            return metrics ?? new List<Metric>();
        }

        private async Task SaveMetricsAsync(List<Metric> metrics)
        {
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            using FileStream fs = File.Create(_jsonFilePath);
            await JsonSerializer.SerializeAsync(fs, metrics, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task<IEnumerable<Metric>> GetAllAsync()
        {
            return await LoadMetricsAsync();
        }

        public async Task<Metric?> GetByIdAsync(Guid id)
        {
            var metrics = await LoadMetricsAsync();
            return metrics.FirstOrDefault(m => m.Id == id);
        }

        public async Task<IEnumerable<Metric>> GetByTypeAsync(string metricType)
        {
            var metrics = await LoadMetricsAsync();
            return metrics.Where(m => m.MetricType.Equals(metricType, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Metric>> GetByCategoryAsync(string category)
        {
            var metrics = await LoadMetricsAsync();
            return metrics.Where(m => m.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Metric> CreateAsync(Metric metric)
        {
            var metrics = await LoadMetricsAsync();
            metric.Id = Guid.NewGuid();
            metric.CreatedDate = DateTime.UtcNow;
            metrics.Add(metric);
            await SaveMetricsAsync(metrics);
            return metric;
        }

        public async Task<Metric> UpdateAsync(Metric metric)
        {
            var metrics = await LoadMetricsAsync();
            var index = metrics.FindIndex(m => m.Id == metric.Id);
            if (index >= 0)
            {
                metrics[index] = metric;
                await SaveMetricsAsync(metrics);
            }
            return metric;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var metrics = await LoadMetricsAsync();
            var metric = metrics.FirstOrDefault(m => m.Id == id);
            if (metric != null)
            {
                metrics.Remove(metric);
                await SaveMetricsAsync(metrics);
                return true;
            }
            return false;
        }
    }
}