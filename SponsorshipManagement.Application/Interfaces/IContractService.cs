using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<ContractDto>> GetAllContractsAsync();
        Task<ContractDto?> GetContractByIdAsync(Guid id);
        Task<IEnumerable<ContractDto>> GetContractsBySponsorIdAsync(string sponsorId);
        Task<IEnumerable<ContractDto>> GetContractsByEventIdAsync(string eventId);
        Task<IEnumerable<ContractDto>> GetActiveContractsAsync();
        Task<ContractDto> CreateContractAsync(ContractDto contractDto);
        Task<ContractDto> UpdateContractAsync(ContractDto contractDto);
        Task<bool> DeleteContractAsync(Guid id);
        Task<bool> ContractExistsAsync(Guid id);
    }
}