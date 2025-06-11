using System.Collections.Generic;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface IContractComplianceReportRepository
    {
        Task<IEnumerable<ContractComplianceReport>> GetAllAsync();
        Task<ContractComplianceReport?> GetByIdAsync(Guid id);
        Task<IEnumerable<ContractComplianceReport>> GetByContractIdAsync(Guid contractId);
        Task<ContractComplianceReport?> GetLatestByContractIdAsync(Guid contractId);
        Task<ContractComplianceReport> CreateAsync(ContractComplianceReport report);
        Task<ContractComplianceReport> UpdateAsync(ContractComplianceReport report);
        Task<bool> DeleteAsync(Guid id);
    }
}