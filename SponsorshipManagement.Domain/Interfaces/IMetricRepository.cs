using System.Collections.Generic;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface IMetricRepository
    {
        Task<IEnumerable<Metric>> GetAllAsync();
        Task<Metric?> GetByIdAsync(Guid id);
        Task<IEnumerable<Metric>> GetByTypeAsync(string metricType);
        Task<IEnumerable<Metric>> GetByCategoryAsync(string category);
        Task<Metric> CreateAsync(Metric metric);
        Task<Metric> UpdateAsync(Metric metric);
        Task<bool> DeleteAsync(Guid id);
    }
}