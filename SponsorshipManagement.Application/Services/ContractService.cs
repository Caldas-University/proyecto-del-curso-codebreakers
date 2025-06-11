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
    public class ContractService : IContractService
    {
        private static List<Contract> _contracts;
        private static readonly string _filePath = "./../SponsorshipManagement.Infrastructure/Data/contracts.json";
        private readonly IEventService _eventService;

        static ContractService()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _contracts = JsonSerializer.Deserialize<List<Contract>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Contract>();
            }
            else
            {
                _contracts = new List<Contract>();
            }
        }

        public ContractService(IEventService eventService)
        {
            _eventService = eventService;
        }

        // Get all contracts
        public Task<IEnumerable<ContractDto>> GetAllContractsAsync()
        {
            return Task.FromResult(_contracts.Select(ToDto));
        }

        // Get contract by id
        public Task<ContractDto?> GetContractByIdAsync(string id)
        {
            var contract = _contracts.FirstOrDefault(c => c.Id.ToString() == id);
            return Task.FromResult(contract == null ? null : ToDto(contract));
        }

        // Create a new contract
        public Task<ContractDto> CreateContractAsync(ContractDto contractDto)
        {
            // Ignorar cualquier Id o CreatedAt recibido
            var contract = new Contract
            {
                Title = contractDto.Title,
                Description = contractDto.Description,
                Value = contractDto.Value,
                StartDate = contractDto.StartDate,
                EndDate = contractDto.EndDate,
                Status = (ContractStatus)contractDto.Status,
                SponsorId = contractDto.SponsorId,
                EventId = contractDto.EventId
            };
            contract.Id = Guid.NewGuid();
            contract.CreatedAt = DateTime.UtcNow;
            contract.UpdatedAt = null;
            _contracts.Add(contract);
            SaveChanges();
            // Update event fund
            UpdateEventFundForContract(contract.EventId);
            return Task.FromResult(ToDto(contract));
        }

        // Update contract y solo permite editar campos de negocio
        public Task<ContractDto?> UpdateContractAsync(string id, ContractDto contractDto)
        {
            var contract = _contracts.FirstOrDefault(c => c.Id.ToString() == id);
            if (contract == null) return Task.FromResult<ContractDto?>(null);
            var previousVersion = JsonSerializer.Serialize(contract);
            // Solo actualizar campos permitidos
            contract.Title = contractDto.Title;
            contract.Description = contractDto.Description;
            contract.Value = contractDto.Value;
            contract.StartDate = contractDto.StartDate;
            contract.EndDate = contractDto.EndDate;
            contract.Status = (ContractStatus)contractDto.Status;
            contract.SponsorId = contractDto.SponsorId;
            contract.EventId = contractDto.EventId;
            contract.UpdatedAt = DateTime.UtcNow;
            SaveChanges();
            // Update event fund
            UpdateEventFundForContract(contract.EventId);
            return Task.FromResult<ContractDto?>(ToDto(contract));
        }

        // Delete contract
        public Task<bool> DeleteContractAsync(string id)
        {
            var contract = _contracts.FirstOrDefault(c => c.Id.ToString() == id);
            if (contract == null) return Task.FromResult(false);
            _contracts.Remove(contract);
            SaveChanges();
            return Task.FromResult(true);
        }

        // Assign benefits to a contract
        public Task<bool> AssignBenefitsAsync(string contractId, string benefits)
        {
            var contract = _contracts.FirstOrDefault(c => c.Id.ToString() == contractId);
            if (contract == null) return Task.FromResult(false);
            contract.Description += $" | Benefits: {benefits}";
            contract.UpdatedAt = DateTime.UtcNow;
            SaveChanges();
            return Task.FromResult(true);
        }

        // Calculate total contributions for an event
        public Task<decimal> CalculateTotalContributionByEventAsync(string eventId)
        {
            return Task.FromResult(_contracts.Where(c => c.EventId == eventId).Sum(c => c.Value));
        }

        // Mapper methods (local, to avoid dependency on API layer)
        private static ContractDto ToDto(Contract contract)
        {
            return new ContractDto
            {
                Id = contract.Id.ToString(),
                Title = contract.Title,
                Description = contract.Description,
                Value = contract.Value,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = (int)contract.Status,
                SponsorId = contract.SponsorId,
                EventId = contract.EventId,
                CreatedAt = contract.CreatedAt,
                UpdatedAt = contract.UpdatedAt
            };
        }
        private static Contract ToEntity(ContractDto dto)
        {
            return new Contract
            {
                Id = Guid.TryParse(dto.Id, out var guid) ? guid : Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Value = dto.Value,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = (ContractStatus)dto.Status,
                SponsorId = dto.SponsorId,
                EventId = dto.EventId,
                CreatedAt = dto.CreatedAt ?? DateTime.UtcNow, // Si es null, asignar ahora
                UpdatedAt = dto.UpdatedAt
            };
        }
        private static void SaveChanges()
        {
            var json = JsonSerializer.Serialize(_contracts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        private void UpdateEventFundForContract(string eventId)
        {
            if (string.IsNullOrEmpty(eventId)) return;
            var total = _contracts.Where(c => c.EventId == eventId).Sum(c => c.Value);
            var eventDto = _eventService.GetEventById(eventId);
            if (eventDto != null)
            {
                eventDto.Fund = total;
                _eventService.UpdateEvent(eventDto);
            }
        }
    }
}