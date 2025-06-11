using SponsorshipManagement.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<ContractDto>> GetAllContractsAsync();
        Task<ContractDto?> GetContractByIdAsync(string id);
        Task<ContractDto> CreateContractAsync(ContractDto contractDto);
        Task<ContractDto?> UpdateContractAsync(string id, ContractDto contractDto);
        Task<bool> DeleteContractAsync(string id);
        Task<bool> AssignBenefitsAsync(string contractId, string benefits);
        Task<decimal> CalculateTotalContributionByEventAsync(string eventId);
    }
}