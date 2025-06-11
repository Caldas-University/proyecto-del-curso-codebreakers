using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class MetricComparisonService : IMetricComparisonService
    {
        private readonly IMetricComparisonRepository _comparisonRepository;
        private readonly IMetricRecordRepository _recordRepository;

        public MetricComparisonService(IMetricComparisonRepository comparisonRepository, IMetricRecordRepository recordRepository)
        {
            _comparisonRepository = comparisonRepository;
            _recordRepository = recordRepository;
        }

        public async Task<IEnumerable<MetricComparisonDto>> GetAllComparisonsAsync()
        {
            var comparisons = await _comparisonRepository.GetAllAsync();
            return comparisons.Select(MapToDto);
        }

        public async Task<MetricComparisonDto?> GetComparisonByIdAsync(Guid id)
        {
            var comparison = await _comparisonRepository.GetByIdAsync(id);
            return comparison != null ? MapToDto(comparison) : null;
        }

        public async Task<IEnumerable<MetricComparisonDto>> GetComparisonsByTypeAsync(string comparisonType)
        {
            var comparisons = await _comparisonRepository.GetByTypeAsync(comparisonType);
            return comparisons.Select(MapToDto);
        }

        public async Task<MetricComparisonDto> CreateComparisonAsync(MetricComparisonDto comparisonDto)
        {
            var comparison = new MetricComparison
            {
                Name = comparisonDto.Name,
                Description = comparisonDto.Description,
                ComparisonType = comparisonDto.ComparisonType,
                MetricRecordIds = comparisonDto.MetricRecordIds,
                StartPeriod = comparisonDto.StartPeriod,
                EndPeriod = comparisonDto.EndPeriod,
                Results = comparisonDto.Results,
                CreatedBy = comparisonDto.CreatedBy
            };

            var createdComparison = await _comparisonRepository.CreateAsync(comparison);
            return MapToDto(createdComparison);
        }

        public async Task<MetricComparisonDto> UpdateComparisonAsync(MetricComparisonDto comparisonDto)
        {
            var comparison = new MetricComparison
            {
                Id = comparisonDto.Id,
                Name = comparisonDto.Name,
                Description = comparisonDto.Description,
                ComparisonType = comparisonDto.ComparisonType,
                MetricRecordIds = comparisonDto.MetricRecordIds,
                StartPeriod = comparisonDto.StartPeriod,
                EndPeriod = comparisonDto.EndPeriod,
                Results = comparisonDto.Results,
                CreatedDate = comparisonDto.CreatedDate,
                CreatedBy = comparisonDto.CreatedBy
            };

            var updatedComparison = await _comparisonRepository.UpdateAsync(comparison);
            return MapToDto(updatedComparison);
        }

        public async Task<bool> DeleteComparisonAsync(Guid id)
        {
            return await _comparisonRepository.DeleteAsync(id);
        }

        public async Task<bool> ComparisonExistsAsync(Guid id)
        {
            var comparison = await _comparisonRepository.GetByIdAsync(id);
            return comparison != null;
        }

        public async Task<MetricComparisonDto> GeneratePeriodComparisonAsync(List<Guid> metricIds, DateTime period1Start, DateTime period1End, DateTime period2Start, DateTime period2End)
        {
            // Implementación básica de comparación entre períodos
            var comparison = new MetricComparison
            {
                Name = $"Comparación de períodos {period1Start:yyyy-MM-dd} vs {period2Start:yyyy-MM-dd}",
                Description = "Comparación automática entre dos períodos de tiempo",
                ComparisonType = "PeriodOverPeriod",
                StartPeriod = period1Start,
                EndPeriod = period2End,
                Results = "{}",
                CreatedBy = "system"
            };

            var createdComparison = await _comparisonRepository.CreateAsync(comparison);
            return MapToDto(createdComparison);
        }

        public async Task<MetricComparisonDto> GenerateEventComparisonAsync(List<Guid> metricIds, string event1Id, string event2Id)
        {
            var comparison = new MetricComparison
            {
                Name = $"Comparación de eventos {event1Id} vs {event2Id}",
                Description = "Comparación automática entre dos eventos",
                ComparisonType = "EventComparison",
                Results = "{}",
                CreatedBy = "system"
            };

            var createdComparison = await _comparisonRepository.CreateAsync(comparison);
            return MapToDto(createdComparison);
        }

        public async Task<MetricComparisonDto> GenerateSponsorComparisonAsync(List<Guid> metricIds, List<string> sponsorDocuments, DateTime startDate, DateTime endDate)
        {
            var comparison = new MetricComparison
            {
                Name = $"Comparación de patrocinadores {string.Join(" vs ", sponsorDocuments)}",
                Description = "Comparación automática entre patrocinadores",
                ComparisonType = "SponsorComparison",
                StartPeriod = startDate,
                EndPeriod = endDate,
                Results = "{}",
                CreatedBy = "system"
            };

            var createdComparison = await _comparisonRepository.CreateAsync(comparison);
            return MapToDto(createdComparison);
        }

        private static MetricComparisonDto MapToDto(MetricComparison comparison)
        {
            return new MetricComparisonDto
            {
                Id = comparison.Id,
                Name = comparison.Name,
                Description = comparison.Description,
                ComparisonType = comparison.ComparisonType,
                MetricRecordIds = comparison.MetricRecordIds,
                StartPeriod = comparison.StartPeriod,
                EndPeriod = comparison.EndPeriod,
                Results = comparison.Results,
                CreatedDate = comparison.CreatedDate,
                CreatedBy = comparison.CreatedBy
            };
        }
    }
}