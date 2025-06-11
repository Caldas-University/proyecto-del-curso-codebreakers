using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private static List<Contract> _contracts = new();
        private static readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/contracts.json";

        static ContractRepository()
        {
            LoadData();
        }

        private static void LoadData()
        {
            if (File.Exists(_jsonFilePath))
            {
                try
                {
                    var json = File.ReadAllText(_jsonFilePath);
                    _contracts = JsonSerializer.Deserialize<List<Contract>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Contract>();
                }
                catch (Exception)
                {
                    _contracts = new List<Contract>();
                }
            }
            else
            {
                _contracts = new List<Contract>();
            }
        }

        private static void SaveData()
        {
            try
            {
                var directory = Path.GetDirectoryName(_jsonFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(_contracts, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_jsonFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving contracts: {ex.Message}");
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await Task.FromResult(_contracts.Any(c => c.Id == id));
        }

        public async Task<bool> ExistsAndIsValidForCommitmentsAsync(Guid id)
        {
            var contract = await GetByIdAsync(id);
            return contract != null && contract.CanHaveCommitments();
        }

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
            return await Task.FromResult(_contracts.AsEnumerable());
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

        public async Task<bool> AddAsync(Contract contract)
        {
            try
            {
                if (contract == null) return false;

                _contracts.Add(contract);
                SaveData();
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
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
