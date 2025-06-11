using System.Collections.Generic;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface IMetricComparisonRepository
    {
        Task<IEnumerable<MetricComparison>> GetAllAsync();
        Task<MetricComparison?> GetByIdAsync(Guid id);
        Task<IEnumerable<MetricComparison>> GetByTypeAsync(string comparisonType);
        Task<MetricComparison> CreateAsync(MetricComparison comparison);
        Task<MetricComparison> UpdateAsync(MetricComparison comparison);
        Task<bool> DeleteAsync(Guid id);
    }
}