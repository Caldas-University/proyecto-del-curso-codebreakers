using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IMetricComparisonService
    {
        Task<IEnumerable<MetricComparisonDto>> GetAllComparisonsAsync();
        Task<MetricComparisonDto?> GetComparisonByIdAsync(Guid id);
        Task<IEnumerable<MetricComparisonDto>> GetComparisonsByTypeAsync(string comparisonType);
        Task<MetricComparisonDto> CreateComparisonAsync(MetricComparisonDto comparisonDto);
        Task<MetricComparisonDto> UpdateComparisonAsync(MetricComparisonDto comparisonDto);
        Task<bool> DeleteComparisonAsync(Guid id);
        Task<bool> ComparisonExistsAsync(Guid id);
        Task<MetricComparisonDto> GeneratePeriodComparisonAsync(List<Guid> metricIds, DateTime period1Start, DateTime period1End, DateTime period2Start, DateTime period2End);
        Task<MetricComparisonDto> GenerateEventComparisonAsync(List<Guid> metricIds, string event1Id, string event2Id);
        Task<MetricComparisonDto> GenerateSponsorComparisonAsync(List<Guid> metricIds, List<string> sponsorDocuments, DateTime startDate, DateTime endDate);
    }
}