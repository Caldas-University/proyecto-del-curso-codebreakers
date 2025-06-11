using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.DTOs;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IAdvertisingBenefitExecutionService
    {
        Task<IEnumerable<AdvertisingBenefitExecutionDto>> GetAllExecutionsAsync();
        Task<AdvertisingBenefitExecutionDto?> GetExecutionByIdAsync(Guid id);
        Task<IEnumerable<AdvertisingBenefitExecutionDto>> GetExecutionsBySponsorAsync(string sponsorDocumentNumber);
        Task<IEnumerable<AdvertisingBenefitExecutionDto>> GetExecutionsByEventAsync(string eventId);
        Task<IEnumerable<AdvertisingBenefitExecutionDto>> GetExecutionsByContractAsync(Guid contractId);
        Task<IEnumerable<AdvertisingBenefitExecutionDto>> GetExecutionsByStatusAsync(string status);
        Task<AdvertisingBenefitExecutionDto> CreateExecutionAsync(AdvertisingBenefitExecutionDto executionDto);
        Task<AdvertisingBenefitExecutionDto> UpdateExecutionAsync(AdvertisingBenefitExecutionDto executionDto);
        Task<bool> DeleteExecutionAsync(Guid id);
        Task<bool> ExecutionExistsAsync(Guid id);
        Task<bool> MarkExecutionAsCompletedAsync(Guid id, string completedBy);
        Task<IEnumerable<VisibilityReportDto>> GetVisibilityReportAsync(string? sponsorDocument = null, string? eventId = null);
    }
}