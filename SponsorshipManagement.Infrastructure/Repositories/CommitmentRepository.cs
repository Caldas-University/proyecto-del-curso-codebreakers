using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using System.Text.Json;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class CommitmentRepository : ICommitmentRepository
    {
        private static List<Commitment> _commitments = new();
        private static readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/commitments.json";

        static CommitmentRepository()
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
                    _commitments = JsonSerializer.Deserialize<List<Commitment>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Commitment>();
                }
                catch (Exception)
                {
                    _commitments = new List<Commitment>();
                }
            }
            else
            {
                _commitments = new List<Commitment>();
                SaveData();
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

                var json = JsonSerializer.Serialize(_commitments, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_jsonFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving commitments: {ex.Message}");
            }
        }

        public async Task<Commitment?> GetByIdAsync(Guid id)
        {
            return await Task.FromResult(_commitments.FirstOrDefault(c => c.Id == id));
        }

        public async Task<IEnumerable<Commitment>> GetByContractIdAsync(Guid contractId)
        {
            return await Task.FromResult(_commitments.Where(c => c.ContractId == contractId));
        }

        public async Task<IEnumerable<Commitment>> GetByStatusAsync(CommitmentStatus status)
        {
            return await Task.FromResult(_commitments.Where(c => c.Status == status));
        }

        public async Task<IEnumerable<Commitment>> GetAllAsync()
        {
            return await Task.FromResult(_commitments.AsEnumerable());
        }

        public async Task<bool> AddAsync(Commitment commitment)
        {
            try
            {
                if (commitment == null) return false;

                _commitments.Add(commitment);
                SaveData();
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAsync(Commitment commitment)
        {
            try
            {
                if (commitment == null) return false;

                var existingIndex = _commitments.FindIndex(c => c.Id == commitment.Id);
                if (existingIndex >= 0)
                {
                    _commitments[existingIndex] = commitment;
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
                var commitment = _commitments.FirstOrDefault(c => c.Id == id);
                if (commitment != null)
                {
                    _commitments.Remove(commitment);
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

        public async Task<bool> ContractExistsAsync(Guid contractId)
        {
            // Simulación de validación de contrato - en una implementación real consultarías el repositorio de contratos
            // Por ahora retorna true si el contractId no es Guid.Empty
            return await Task.FromResult(contractId != Guid.Empty);
        }

        public async Task<IEnumerable<Commitment>> GetOverdueCommitmentsAsync()
        {
            var currentDate = DateTime.UtcNow;
            return await Task.FromResult(_commitments.Where(c =>
                c.DueDate < currentDate &&
                c.Status == CommitmentStatus.Pending));
        }
    }
}
