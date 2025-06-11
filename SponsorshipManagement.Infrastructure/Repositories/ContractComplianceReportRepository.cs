using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class ContractComplianceReportRepository : IContractComplianceReportRepository
    {
        private readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/contract_compliance_reports.json";

        private async Task<List<ContractComplianceReport>> LoadReportsAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return new List<ContractComplianceReport>();

            using FileStream fs = File.OpenRead(_jsonFilePath);
            var reports = await JsonSerializer.DeserializeAsync<List<ContractComplianceReport>>(fs);
            return reports ?? new List<ContractComplianceReport>();
        }

        private async Task SaveReportsAsync(List<ContractComplianceReport> reports)
        {
            var directory = Path.GetDirectoryName(_jsonFilePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory!);

            using FileStream fs = File.Create(_jsonFilePath);
            await JsonSerializer.SerializeAsync(fs, reports, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task<IEnumerable<ContractComplianceReport>> GetAllAsync()
        {
            return await LoadReportsAsync();
        }

        public async Task<ContractComplianceReport?> GetByIdAsync(Guid id)
        {
            var reports = await LoadReportsAsync();
            return reports.FirstOrDefault(r => r.Id == id);
        }

        public async Task<IEnumerable<ContractComplianceReport>> GetByContractIdAsync(Guid contractId)
        {
            var reports = await LoadReportsAsync();
            return reports.Where(r => r.ContractId == contractId).OrderByDescending(r => r.ReportDate);
        }

        public async Task<ContractComplianceReport?> GetLatestByContractIdAsync(Guid contractId)
        {
            var reports = await LoadReportsAsync();
            return reports
                .Where(r => r.ContractId == contractId)
                .OrderByDescending(r => r.ReportDate)
                .FirstOrDefault();
        }

        public async Task<ContractComplianceReport> CreateAsync(ContractComplianceReport report)
        {
            var reports = await LoadReportsAsync();
            reports.Add(report);
            await SaveReportsAsync(reports);
            return report;
        }

        public async Task<ContractComplianceReport> UpdateAsync(ContractComplianceReport report)
        {
            var reports = await LoadReportsAsync();
            var index = reports.FindIndex(r => r.Id == report.Id);
            if (index >= 0)
            {
                reports[index] = report;
                await SaveReportsAsync(reports);
            }
            return report;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var reports = await LoadReportsAsync();
            var report = reports.FirstOrDefault(r => r.Id == id);
            if (report != null)
            {
                reports.Remove(report);
                await SaveReportsAsync(reports);
                return true;
            }
            return false;
        }
    }
}