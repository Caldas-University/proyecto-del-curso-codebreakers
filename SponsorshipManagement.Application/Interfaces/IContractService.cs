using SponsorshipManagement.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<ContractDto>> GetAllContractsAsync();
        Task<ContractDto> CreateContractAsync(ContractDto contractDto);
        Task<ContractDto?> UpdateContractAsync(Guid id, ContractDto contractDto); // Changed string id to Guid id
        Task<ContractDto> UpdateContractAsync(ContractDto contractDto);
        Task<bool> AssignBenefitsAsync(Guid contractId, string benefits); // Changed string contractId to Guid contractId
        Task<decimal> CalculateTotalContributionByEventAsync(string eventId);


        Task<ContractDto?> GetContractByIdAsync(Guid id);
        Task<IEnumerable<ContractDto>> GetContractsBySponsorIdAsync(string sponsorId);
        Task<IEnumerable<ContractDto>> GetContractsByEventIdAsync(string eventId);
        Task<IEnumerable<ContractDto>> GetActiveContractsAsync();
        Task<bool> DeleteContractAsync(Guid id);
        Task<bool> ContractExistsAsync(Guid id);
    }
}