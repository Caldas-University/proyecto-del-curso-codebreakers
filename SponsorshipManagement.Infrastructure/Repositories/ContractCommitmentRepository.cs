using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class ContractCommitmentRepository : IContractCommitmentRepository
    {
        private readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/contract_commitments.json";

        private async Task<List<ContractCommitment>> LoadCommitmentsAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return new List<ContractCommitment>();

            using FileStream fs = File.OpenRead(_jsonFilePath);
            var commitments = await JsonSerializer.DeserializeAsync<List<ContractCommitment>>(fs);
            return commitments ?? new List<ContractCommitment>();
        }

        private async Task SaveCommitmentsAsync(List<ContractCommitment> commitments)
        {
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            using FileStream fs = File.Create(_jsonFilePath);
            await JsonSerializer.SerializeAsync(fs, commitments, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task<IEnumerable<ContractCommitment>> GetAllAsync()
        {
            return await LoadCommitmentsAsync();
        }

        public async Task<ContractCommitment?> GetByIdAsync(Guid id)
        {
            var commitments = await LoadCommitmentsAsync();
            return commitments.FirstOrDefault(c => c.Id == id);
        }

        public async Task<IEnumerable<ContractCommitment>> GetByContractIdAsync(Guid contractId)
        {
            var commitments = await LoadCommitmentsAsync();
            return commitments.Where(c => c.ContractId == contractId);
        }

        public async Task<IEnumerable<ContractCommitment>> GetByStatusAsync(CommitmentStatus status)
        {
            var commitments = await LoadCommitmentsAsync();
            return commitments.Where(c => c.Status == status);
        }

        public async Task<IEnumerable<ContractCommitment>> GetOverdueCommitmentsAsync()
        {
            var commitments = await LoadCommitmentsAsync();
            return commitments.Where(c => c.IsOverdue());
        }

        public async Task<ContractCommitment> CreateAsync(ContractCommitment commitment)
        {
            var commitments = await LoadCommitmentsAsync();
            commitments.Add(commitment);
            await SaveCommitmentsAsync(commitments);
            return commitment;
        }

        public async Task<ContractCommitment> UpdateAsync(ContractCommitment commitment)
        {
            var commitments = await LoadCommitmentsAsync();
            var index = commitments.FindIndex(c => c.Id == commitment.Id);
            if (index >= 0)
            {
                commitment.UpdatedAt = DateTime.UtcNow;
                commitments[index] = commitment;
                await SaveCommitmentsAsync(commitments);
            }
            return commitment;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var commitments = await LoadCommitmentsAsync();
            var commitment = commitments.FirstOrDefault(c => c.Id == id);
            if (commitment != null)
            {
                commitments.Remove(commitment);
                await SaveCommitmentsAsync(commitments);
                return true;
            }
            return false;
        }
    }
}