using SponsorshipManagement.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IContractHistoryService
    {
        Task<IEnumerable<ContractHistoryDto>> GetHistoryByContractIdAsync(string contractId);
        Task<ContractHistoryDto> AddHistoryAsync(ContractHistoryDto historyDto);
    }
}