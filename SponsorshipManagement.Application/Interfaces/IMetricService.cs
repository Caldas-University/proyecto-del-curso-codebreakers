using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IMetricService
    {
        Task<IEnumerable<MetricDto>> GetAllMetricsAsync();
        Task<MetricDto?> GetMetricByIdAsync(Guid id);
        Task<IEnumerable<MetricDto>> GetMetricsByTypeAsync(string metricType);
        Task<IEnumerable<MetricDto>> GetMetricsByCategoryAsync(string category);
        Task<MetricDto> CreateMetricAsync(MetricDto metricDto);
        Task<MetricDto> UpdateMetricAsync(MetricDto metricDto);
        Task<bool> DeleteMetricAsync(Guid id);
        Task<bool> MetricExistsAsync(Guid id);
    }
}