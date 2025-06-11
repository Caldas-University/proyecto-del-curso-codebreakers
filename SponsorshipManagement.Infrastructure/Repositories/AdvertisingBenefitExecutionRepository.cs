using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class AdvertisingBenefitExecutionRepository : IAdvertisingBenefitExecutionRepository
    {
        private readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/advertising_benefit_executions.json";

        private async Task<List<AdvertisingBenefitExecution>> LoadExecutionsAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return new List<AdvertisingBenefitExecution>();

            using FileStream fs = File.OpenRead(_jsonFilePath);
            var executions = await JsonSerializer.DeserializeAsync<List<AdvertisingBenefitExecution>>(fs);
            return executions ?? new List<AdvertisingBenefitExecution>();
        }

        private async Task SaveExecutionsAsync(List<AdvertisingBenefitExecution> executions)
        {
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            using FileStream fs = File.Create(_jsonFilePath);
            await JsonSerializer.SerializeAsync(fs, executions, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task<IEnumerable<AdvertisingBenefitExecution>> GetAllAsync()
        {
            return await LoadExecutionsAsync();
        }

        public async Task<AdvertisingBenefitExecution?> GetByIdAsync(Guid id)
        {
            var executions = await LoadExecutionsAsync();
            return executions.FirstOrDefault(e => e.Id == id);
        }

        public async Task<IEnumerable<AdvertisingBenefitExecution>> GetBySponsorAsync(string sponsorDocumentNumber)
        {
            var executions = await LoadExecutionsAsync();
            return executions.Where(e => e.SponsorDocumentNumber == sponsorDocumentNumber);
        }

        public async Task<IEnumerable<AdvertisingBenefitExecution>> GetByEventAsync(string eventId)
        {
            var executions = await LoadExecutionsAsync();
            return executions.Where(e => e.EventId == eventId);
        }

        public async Task<IEnumerable<AdvertisingBenefitExecution>> GetByContractAsync(Guid contractId)
        {
            var executions = await LoadExecutionsAsync();
            return executions.Where(e => e.SponsorshipContractId == contractId);
        }

        public async Task<IEnumerable<AdvertisingBenefitExecution>> GetByStatusAsync(string status)
        {
            var executions = await LoadExecutionsAsync();
            return executions.Where(e => e.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<AdvertisingBenefitExecution> CreateAsync(AdvertisingBenefitExecution execution)
        {
            var executions = await LoadExecutionsAsync();
            execution.Id = Guid.NewGuid();
            execution.CreatedDate = DateTime.UtcNow;
            executions.Add(execution);
            await SaveExecutionsAsync(executions);
            return execution;
        }

        public async Task<AdvertisingBenefitExecution> UpdateAsync(AdvertisingBenefitExecution execution)
        {
            var executions = await LoadExecutionsAsync();
            var index = executions.FindIndex(e => e.Id == execution.Id);
            if (index >= 0)
            {
                executions[index] = execution;
                await SaveExecutionsAsync(executions);
            }
            return execution;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var executions = await LoadExecutionsAsync();
            var execution = executions.FirstOrDefault(e => e.Id == id);
            if (execution != null)
            {
                executions.Remove(execution);
                await SaveExecutionsAsync(executions);
                return true;
            }
            return false;
        }
    }
}