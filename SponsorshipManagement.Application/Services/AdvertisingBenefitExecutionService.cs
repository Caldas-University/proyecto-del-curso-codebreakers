using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class AdvertisingBenefitExecutionService : IAdvertisingBenefitExecutionService
    {
        private readonly IAdvertisingBenefitExecutionRepository _executionRepository;

        public AdvertisingBenefitExecutionService(IAdvertisingBenefitExecutionRepository executionRepository)
        {
            _executionRepository = executionRepository;
        }

        public async Task<IEnumerable<AdvertisingBenefitExecutionDto>> GetAllExecutionsAsync()
        {
            var executions = await _executionRepository.GetAllAsync();
            return executions.Select(MapToDto);
        }

        public async Task<AdvertisingBenefitExecutionDto?> GetExecutionByIdAsync(Guid id)
        {
            var execution = await _executionRepository.GetByIdAsync(id);
            return execution != null ? MapToDto(execution) : null;
        }

        public async Task<IEnumerable<AdvertisingBenefitExecutionDto>> GetExecutionsBySponsorAsync(string sponsorDocumentNumber)
        {
            var executions = await _executionRepository.GetBySponsorAsync(sponsorDocumentNumber);
            return executions.Select(MapToDto);
        }

        public async Task<IEnumerable<AdvertisingBenefitExecutionDto>> GetExecutionsByEventAsync(string eventId)
        {
            var executions = await _executionRepository.GetByEventAsync(eventId);
            return executions.Select(MapToDto);
        }

        public async Task<IEnumerable<AdvertisingBenefitExecutionDto>> GetExecutionsByContractAsync(Guid contractId)
        {
            var executions = await _executionRepository.GetByContractAsync(contractId);
            return executions.Select(MapToDto);
        }

        public async Task<IEnumerable<AdvertisingBenefitExecutionDto>> GetExecutionsByStatusAsync(string status)
        {
            var executions = await _executionRepository.GetByStatusAsync(status);
            return executions.Select(MapToDto);
        }

        public async Task<AdvertisingBenefitExecutionDto> CreateExecutionAsync(AdvertisingBenefitExecutionDto executionDto)
        {
            var execution = new AdvertisingBenefitExecution
            {
                SponsorshipContractId = executionDto.SponsorshipContractId,
                AdvertisingBenefitId = executionDto.AdvertisingBenefitId,
                SponsorDocumentNumber = executionDto.SponsorDocumentNumber,
                EventId = executionDto.EventId,
                ExecutionDate = executionDto.ExecutionDate,
                Status = executionDto.Status,
                ExecutionDetails = executionDto.ExecutionDetails,
                MediaFiles = executionDto.MediaFiles,
                ActualCost = executionDto.ActualCost,
                ExecutedBy = executionDto.ExecutedBy,
                Notes = executionDto.Notes
            };

            var createdExecution = await _executionRepository.CreateAsync(execution);
            return MapToDto(createdExecution);
        }

        public async Task<AdvertisingBenefitExecutionDto> UpdateExecutionAsync(AdvertisingBenefitExecutionDto executionDto)
        {
            var execution = new AdvertisingBenefitExecution
            {
                Id = executionDto.Id,
                SponsorshipContractId = executionDto.SponsorshipContractId,
                AdvertisingBenefitId = executionDto.AdvertisingBenefitId,
                SponsorDocumentNumber = executionDto.SponsorDocumentNumber,
                EventId = executionDto.EventId,
                ExecutionDate = executionDto.ExecutionDate,
                Status = executionDto.Status,
                ExecutionDetails = executionDto.ExecutionDetails,
                MediaFiles = executionDto.MediaFiles,
                ActualCost = executionDto.ActualCost,
                ExecutedBy = executionDto.ExecutedBy,
                CreatedDate = executionDto.CreatedDate,
                CompletedDate = executionDto.CompletedDate,
                Notes = executionDto.Notes
            };

            var updatedExecution = await _executionRepository.UpdateAsync(execution);
            return MapToDto(updatedExecution);
        }

        public async Task<bool> DeleteExecutionAsync(Guid id)
        {
            return await _executionRepository.DeleteAsync(id);
        }

        public async Task<bool> ExecutionExistsAsync(Guid id)
        {
            var execution = await _executionRepository.GetByIdAsync(id);
            return execution != null;
        }

        public async Task<bool> MarkExecutionAsCompletedAsync(Guid id, string completedBy)
        {
            var execution = await _executionRepository.GetByIdAsync(id);
            if (execution == null) return false;

            execution.Status = "Ejecutado";
            execution.CompletedDate = DateTime.UtcNow;
            execution.ExecutedBy = completedBy;

            await _executionRepository.UpdateAsync(execution);
            return true;
        }

        private static AdvertisingBenefitExecutionDto MapToDto(AdvertisingBenefitExecution execution)
        {
            return new AdvertisingBenefitExecutionDto
            {
                Id = execution.Id,
                SponsorshipContractId = execution.SponsorshipContractId,
                AdvertisingBenefitId = execution.AdvertisingBenefitId,
                SponsorDocumentNumber = execution.SponsorDocumentNumber,
                EventId = execution.EventId,
                ExecutionDate = execution.ExecutionDate,
                Status = execution.Status,
                ExecutionDetails = execution.ExecutionDetails,
                MediaFiles = execution.MediaFiles,
                ActualCost = execution.ActualCost,
                ExecutedBy = execution.ExecutedBy,
                CreatedDate = execution.CreatedDate,
                CompletedDate = execution.CompletedDate,
                Notes = execution.Notes
            };
        }
    }
}