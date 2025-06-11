using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IMetricRecordService
    {
        Task<IEnumerable<MetricRecordDto>> GetAllRecordsAsync();
        Task<MetricRecordDto?> GetRecordByIdAsync(Guid id);
        Task<IEnumerable<MetricRecordDto>> GetRecordsByMetricAsync(Guid metricId);
        Task<IEnumerable<MetricRecordDto>> GetRecordsByExecutionAsync(Guid executionId);
        Task<IEnumerable<MetricRecordDto>> GetRecordsBySponsorAsync(string sponsorDocumentNumber);
        Task<IEnumerable<MetricRecordDto>> GetRecordsByEventAsync(string eventId);
        Task<IEnumerable<MetricRecordDto>> GetRecordsByPeriodAsync(DateTime startDate, DateTime endDate);
        Task<MetricRecordDto> CreateRecordAsync(MetricRecordDto recordDto);
        Task<MetricRecordDto> UpdateRecordAsync(MetricRecordDto recordDto);
        Task<bool> DeleteRecordAsync(Guid id);
        Task<bool> RecordExistsAsync(Guid id);
        Task<decimal> CalculateAverageAsync(Guid metricId, DateTime startDate, DateTime endDate);
        Task<decimal> CalculateTotalAsync(Guid metricId, DateTime startDate, DateTime endDate);
    }
}