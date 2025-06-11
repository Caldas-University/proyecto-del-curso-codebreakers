using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class MetricRecordService : IMetricRecordService
    {
        private readonly IMetricRecordRepository _recordRepository;

        public MetricRecordService(IMetricRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public async Task<IEnumerable<MetricRecordDto>> GetAllRecordsAsync()
        {
            var records = await _recordRepository.GetAllAsync();
            return records.Select(MapToDto);
        }

        public async Task<MetricRecordDto?> GetRecordByIdAsync(Guid id)
        {
            var record = await _recordRepository.GetByIdAsync(id);
            return record != null ? MapToDto(record) : null;
        }

        public async Task<IEnumerable<MetricRecordDto>> GetRecordsByMetricAsync(Guid metricId)
        {
            var records = await _recordRepository.GetByMetricAsync(metricId);
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<MetricRecordDto>> GetRecordsByExecutionAsync(Guid executionId)
        {
            var records = await _recordRepository.GetByExecutionAsync(executionId);
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<MetricRecordDto>> GetRecordsBySponsorAsync(string sponsorDocumentNumber)
        {
            var records = await _recordRepository.GetBySponsorAsync(sponsorDocumentNumber);
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<MetricRecordDto>> GetRecordsByEventAsync(string eventId)
        {
            var records = await _recordRepository.GetByEventAsync(eventId);
            return records.Select(MapToDto);
        }

        public async Task<IEnumerable<MetricRecordDto>> GetRecordsByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            var records = await _recordRepository.GetByPeriodAsync(startDate, endDate);
            return records.Select(MapToDto);
        }

        public async Task<MetricRecordDto> CreateRecordAsync(MetricRecordDto recordDto)
        {
            var record = new MetricRecord
            {
                MetricId = recordDto.MetricId,
                AdvertisingBenefitExecutionId = recordDto.AdvertisingBenefitExecutionId,
                SponsorDocumentNumber = recordDto.SponsorDocumentNumber,
                EventId = recordDto.EventId,
                Value = recordDto.Value,
                MeasurementDate = recordDto.MeasurementDate,
                Source = recordDto.Source,
                Period = recordDto.Period,
                Notes = recordDto.Notes,
                CreatedBy = recordDto.CreatedBy
            };

            var createdRecord = await _recordRepository.CreateAsync(record);
            return MapToDto(createdRecord);
        }

        public async Task<MetricRecordDto> UpdateRecordAsync(MetricRecordDto recordDto)
        {
            var record = new MetricRecord
            {
                Id = recordDto.Id,
                MetricId = recordDto.MetricId,
                AdvertisingBenefitExecutionId = recordDto.AdvertisingBenefitExecutionId,
                SponsorDocumentNumber = recordDto.SponsorDocumentNumber,
                EventId = recordDto.EventId,
                Value = recordDto.Value,
                MeasurementDate = recordDto.MeasurementDate,
                Source = recordDto.Source,
                Period = recordDto.Period,
                Notes = recordDto.Notes,
                CreatedDate = recordDto.CreatedDate,
                CreatedBy = recordDto.CreatedBy
            };

            var updatedRecord = await _recordRepository.UpdateAsync(record);
            return MapToDto(updatedRecord);
        }

        public async Task<bool> DeleteRecordAsync(Guid id)
        {
            return await _recordRepository.DeleteAsync(id);
        }

        public async Task<bool> RecordExistsAsync(Guid id)
        {
            var record = await _recordRepository.GetByIdAsync(id);
            return record != null;
        }

        public async Task<decimal> CalculateAverageAsync(Guid metricId, DateTime startDate, DateTime endDate)
        {
            var records = await _recordRepository.GetByMetricAsync(metricId);
            var filteredRecords = records.Where(r => r.MeasurementDate >= startDate && r.MeasurementDate <= endDate);
            return filteredRecords.Any() ? filteredRecords.Average(r => r.Value) : 0;
        }

        public async Task<decimal> CalculateTotalAsync(Guid metricId, DateTime startDate, DateTime endDate)
        {
            var records = await _recordRepository.GetByMetricAsync(metricId);
            var filteredRecords = records.Where(r => r.MeasurementDate >= startDate && r.MeasurementDate <= endDate);
            return filteredRecords.Sum(r => r.Value);
        }

        private static MetricRecordDto MapToDto(MetricRecord record)
        {
            return new MetricRecordDto
            {
                Id = record.Id,
                MetricId = record.MetricId,
                AdvertisingBenefitExecutionId = record.AdvertisingBenefitExecutionId,
                SponsorDocumentNumber = record.SponsorDocumentNumber,
                EventId = record.EventId,
                Value = record.Value,
                MeasurementDate = record.MeasurementDate,
                Source = record.Source,
                Period = record.Period,
                Notes = record.Notes,
                CreatedDate = record.CreatedDate,
                CreatedBy = record.CreatedBy
            };
        }
    }
}