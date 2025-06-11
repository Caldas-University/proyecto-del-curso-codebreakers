using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class MetricService : IMetricService
    {
        private readonly IMetricRepository _metricRepository;

        public MetricService(IMetricRepository metricRepository)
        {
            _metricRepository = metricRepository;
        }

        public async Task<IEnumerable<MetricDto>> GetAllMetricsAsync()
        {
            var metrics = await _metricRepository.GetAllAsync();
            return metrics.Select(MapToDto);
        }

        public async Task<MetricDto?> GetMetricByIdAsync(Guid id)
        {
            var metric = await _metricRepository.GetByIdAsync(id);
            return metric != null ? MapToDto(metric) : null;
        }

        public async Task<IEnumerable<MetricDto>> GetMetricsByTypeAsync(string metricType)
        {
            var metrics = await _metricRepository.GetByTypeAsync(metricType);
            return metrics.Select(MapToDto);
        }

        public async Task<IEnumerable<MetricDto>> GetMetricsByCategoryAsync(string category)
        {
            var metrics = await _metricRepository.GetByCategoryAsync(category);
            return metrics.Select(MapToDto);
        }

        public async Task<MetricDto> CreateMetricAsync(MetricDto metricDto)
        {
            var metric = new Metric
            {
                Name = metricDto.Name,
                Description = metricDto.Description,
                MetricType = metricDto.MetricType,
                Category = metricDto.Category,
                Unit = metricDto.Unit,
                IsActive = metricDto.IsActive
            };

            var createdMetric = await _metricRepository.CreateAsync(metric);
            return MapToDto(createdMetric);
        }

        public async Task<MetricDto> UpdateMetricAsync(MetricDto metricDto)
        {
            var metric = new Metric
            {
                Id = metricDto.Id,
                Name = metricDto.Name,
                Description = metricDto.Description,
                MetricType = metricDto.MetricType,
                Category = metricDto.Category,
                Unit = metricDto.Unit,
                IsActive = metricDto.IsActive,
                CreatedDate = metricDto.CreatedDate
            };

            var updatedMetric = await _metricRepository.UpdateAsync(metric);
            return MapToDto(updatedMetric);
        }

        public async Task<bool> DeleteMetricAsync(Guid id)
        {
            return await _metricRepository.DeleteAsync(id);
        }

        public async Task<bool> MetricExistsAsync(Guid id)
        {
            var metric = await _metricRepository.GetByIdAsync(id);
            return metric != null;
        }

        private static MetricDto MapToDto(Metric metric)
        {
            return new MetricDto
            {
                Id = metric.Id,
                Name = metric.Name,
                Description = metric.Description,
                MetricType = metric.MetricType,
                Category = metric.Category,
                Unit = metric.Unit,
                IsActive = metric.IsActive,
                CreatedDate = metric.CreatedDate
            };
        }
    }
}