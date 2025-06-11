using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class MetricRecordRepository : IMetricRecordRepository
    {
        private readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/metric_records.json";

        private async Task<List<MetricRecord>> LoadRecordsAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return new List<MetricRecord>();

            using FileStream fs = File.OpenRead(_jsonFilePath);
            var records = await JsonSerializer.DeserializeAsync<List<MetricRecord>>(fs);
            return records ?? new List<MetricRecord>();
        }

        private async Task SaveRecordsAsync(List<MetricRecord> records)
        {
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            using FileStream fs = File.Create(_jsonFilePath);
            await JsonSerializer.SerializeAsync(fs, records, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task<IEnumerable<MetricRecord>> GetAllAsync()
        {
            return await LoadRecordsAsync();
        }

        public async Task<MetricRecord?> GetByIdAsync(Guid id)
        {
            var records = await LoadRecordsAsync();
            return records.FirstOrDefault(r => r.Id == id);
        }

        public async Task<IEnumerable<MetricRecord>> GetByMetricAsync(Guid metricId)
        {
            var records = await LoadRecordsAsync();
            return records.Where(r => r.MetricId == metricId);
        }

        public async Task<IEnumerable<MetricRecord>> GetByExecutionAsync(Guid executionId)
        {
            var records = await LoadRecordsAsync();
            return records.Where(r => r.AdvertisingBenefitExecutionId == executionId);
        }

        public async Task<IEnumerable<MetricRecord>> GetBySponsorAsync(string sponsorDocumentNumber)
        {
            var records = await LoadRecordsAsync();
            return records.Where(r => r.SponsorDocumentNumber == sponsorDocumentNumber);
        }

        public async Task<IEnumerable<MetricRecord>> GetByEventAsync(string eventId)
        {
            var records = await LoadRecordsAsync();
            return records.Where(r => r.EventId == eventId);
        }

        public async Task<IEnumerable<MetricRecord>> GetByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var records = await LoadRecordsAsync();
            return records.Where(r => r.MeasurementDate >= startDate && r.MeasurementDate <= endDate);
        }

        public async Task<MetricRecord> CreateAsync(MetricRecord record)
        {
            var records = await LoadRecordsAsync();
            record.Id = Guid.NewGuid();
            record.CreatedDate = DateTime.UtcNow;
            records.Add(record);
            await SaveRecordsAsync(records);
            return record;
        }

        public async Task<MetricRecord> UpdateAsync(MetricRecord record)
        {
            var records = await LoadRecordsAsync();
            var index = records.FindIndex(r => r.Id == record.Id);
            if (index >= 0)
            {
                records[index] = record;
                await SaveRecordsAsync(records);
            }
            return record;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var records = await LoadRecordsAsync();
            var record = records.FirstOrDefault(r => r.Id == id);
            if (record != null)
            {
                records.Remove(record);
                await SaveRecordsAsync(records);
                return true;
            }
            return false;
        }
    }
}