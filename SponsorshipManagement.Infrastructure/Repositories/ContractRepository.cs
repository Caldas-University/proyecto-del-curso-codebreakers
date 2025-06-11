using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/contracts.json";

        private async Task<List<Contract>> LoadContractsAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return new List<Contract>();

            using FileStream fs = File.OpenRead(_jsonFilePath);
            var contracts = await JsonSerializer.DeserializeAsync<List<Contract>>(fs);
            return contracts ?? new List<Contract>();
        }

        private async Task SaveContractsAsync(List<Contract> contracts)
        {
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            using FileStream fs = File.Create(_jsonFilePath);
            await JsonSerializer.SerializeAsync(fs, contracts, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await LoadContractsAsync();
        }

        public async Task<Contract?> GetByIdAsync(Guid id)
        {
            var contracts = await LoadContractsAsync();
            return contracts.FirstOrDefault(c => c.Id == id);
        }

        public async Task<IEnumerable<Contract>> GetBySponsorIdAsync(string sponsorId)
        {
            var contracts = await LoadContractsAsync();
            return contracts.Where(c => c.SponsorId == sponsorId);
        }

        public async Task<IEnumerable<Contract>> GetByEventIdAsync(string eventId)
        {
            var contracts = await LoadContractsAsync();
            return contracts.Where(c => c.EventId == eventId);
        }

        public async Task<IEnumerable<Contract>> GetByStatusAsync(ContractStatus status)
        {
            var contracts = await LoadContractsAsync();
            return contracts.Where(c => c.Status == status);
        }

        public async Task<IEnumerable<Contract>> GetActiveContractsAsync()
        {
            var contracts = await LoadContractsAsync();
            return contracts.Where(c => c.IsActive());
        }

        public async Task<Contract> CreateAsync(Contract contract)
        {
            var contracts = await LoadContractsAsync();
            contracts.Add(contract);
            await SaveContractsAsync(contracts);
            return contract;
        }

        public async Task<Contract> UpdateAsync(Contract contract)
        {
            var contracts = await LoadContractsAsync();
            var index = contracts.FindIndex(c => c.Id == contract.Id);
            if (index >= 0)
            {
                contract.UpdatedAt = DateTime.UtcNow;
                contracts[index] = contract;
                await SaveContractsAsync(contracts);
            }
            return contract;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var contracts = await LoadContractsAsync();
            var contract = contracts.FirstOrDefault(c => c.Id == id);
            if (contract != null)
            {
                contracts.Remove(contract);
                await SaveContractsAsync(contracts);
                return true;
            }
            return false;
        }
    }
}