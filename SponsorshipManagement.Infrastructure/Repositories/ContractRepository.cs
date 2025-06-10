using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using System.Text.Json;

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
                    InitializeWithSampleData();
                }
            }
            else
            {
                _contracts = new List<Contract>();
                InitializeWithSampleData();
            }
        }

        private static void InitializeWithSampleData()
        {
            // Crear contratos de ejemplo que coincidan con los IDs usados en commitments.json
            _contracts.AddRange(new[]
            {
                new Contract
                {
                    Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
                    Title = "Contrato de Patrocinio General",
                    Description = "Contrato para patrocinio de eventos corporativos",
                    Value = 50000.00m,
                    StartDate = DateTime.Parse("2024-01-01"),
                    EndDate = DateTime.Parse("2024-12-31"),
                    Status = ContractStatus.Active,
                    SponsorId = "sponsor-001",
                    EventId = "event-001",
                    CreatedAt = DateTime.Parse("2024-01-01T10:00:00Z")
                },
                new Contract
                {
                    Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174001"),
                    Title = "Contrato de Patrocinio Deportivo",
                    Description = "Contrato específico para eventos deportivos y competencias",
                    Value = 75000.00m,
                    StartDate = DateTime.Parse("2024-06-01"),
                    EndDate = DateTime.Parse("2024-12-31"),
                    Status = ContractStatus.Signed,
                    SponsorId = "sponsor-002",
                    EventId = "event-002",
                    CreatedAt = DateTime.Parse("2024-05-15T14:30:00Z")
                },
                new Contract
                {
                    Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174002"),
                    Title = "Contrato Cancelado",
                    Description = "Contrato de ejemplo cancelado para pruebas",
                    Value = 25000.00m,
                    StartDate = DateTime.Parse("2024-03-01"),
                    EndDate = DateTime.Parse("2024-06-30"),
                    Status = ContractStatus.Cancelled,
                    SponsorId = "sponsor-003",
                    EventId = "event-003",
                    CreatedAt = DateTime.Parse("2024-02-15T09:00:00Z")
                }
            });

            SaveData();
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

        public async Task<Contract?> GetByIdAsync(Guid id)
        {
            return await Task.FromResult(_contracts.FirstOrDefault(c => c.Id == id));
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

        public async Task<IEnumerable<Contract>> GetAllAsync()
        {
            return await Task.FromResult(_contracts.AsEnumerable());
        }

        public async Task<IEnumerable<Contract>> GetActiveContractsAsync()
        {
            return await Task.FromResult(_contracts.Where(c => c.IsActive()));
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

        public async Task<bool> UpdateAsync(Contract contract)
        {
            try
            {
                if (contract == null) return false;

                var existingIndex = _contracts.FindIndex(c => c.Id == contract.Id);
                if (existingIndex >= 0)
                {
                    _contracts[existingIndex] = contract;
                    SaveData();
                    return await Task.FromResult(true);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var contract = _contracts.FirstOrDefault(c => c.Id == id);
                if (contract != null)
                {
                    _contracts.Remove(contract);
                    SaveData();
                    return await Task.FromResult(true);
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
