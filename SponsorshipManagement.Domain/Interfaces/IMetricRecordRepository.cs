using System.Collections.Generic;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface IMetricRecordRepository
    {
        Task<IEnumerable<MetricRecord>> GetAllAsync();
        Task<MetricRecord?> GetByIdAsync(Guid id);
        Task<IEnumerable<MetricRecord>> GetByMetricAsync(Guid metricId);
        Task<IEnumerable<MetricRecord>> GetByExecutionAsync(Guid executionId);
        Task<IEnumerable<MetricRecord>> GetBySponsorAsync(string sponsorDocumentNumber);
        Task<IEnumerable<MetricRecord>> GetByEventAsync(string eventId);
        Task<IEnumerable<MetricRecord>> GetByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<MetricRecord> CreateAsync(MetricRecord record);
        Task<MetricRecord> UpdateAsync(MetricRecord record);
        Task<bool> DeleteAsync(Guid id);
    }
}