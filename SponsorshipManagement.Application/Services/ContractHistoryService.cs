using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Services
{
    public class ContractHistoryService : IContractHistoryService
    {
        private static List<ContractHistory> _history;
        private static readonly string _filePath = "./../SponsorshipManagement.Infrastructure/Data/contractHistory.json";

        static ContractHistoryService()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _history = JsonSerializer.Deserialize<List<ContractHistory>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<ContractHistory>();
            }
            else
            {
                _history = new List<ContractHistory>();
            }
        }

        // Get contract history by contract id
        public Task<IEnumerable<ContractHistoryDto>> GetHistoryByContractIdAsync(string contractId)
        {
            return Task.FromResult(_history.Where(h => h.ContractId == contractId).Select(ToDto));
        }

        // Add a new history entry
        public Task<ContractHistoryDto> AddHistoryAsync(ContractHistoryDto historyDto)
        {
            var history = ToEntity(historyDto);
            history.Id = Guid.NewGuid().ToString();
            history.Timestamp = DateTime.UtcNow;
            _history.Add(history);
            SaveChanges();
            return Task.FromResult(ToDto(history));
        }

        // Mapper methods (local)
        private static ContractHistoryDto ToDto(ContractHistory history)
        {
            return new ContractHistoryDto
            {
                Id = history.Id,
                ContractId = history.ContractId,
                Action = history.Action,
                Timestamp = history.Timestamp,
                PreviousVersionJson = history.PreviousVersionJson
            };
        }
        private static ContractHistory ToEntity(ContractHistoryDto dto)
        {
            return new ContractHistory
            {
                Id = dto.Id,
                ContractId = dto.ContractId,
                Action = dto.Action,
                Timestamp = dto.Timestamp,
                PreviousVersionJson = dto.PreviousVersionJson
            };
        }
        private static void SaveChanges()
        {
            var json = JsonSerializer.Serialize(_history, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}