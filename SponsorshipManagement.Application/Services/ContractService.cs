using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class ContractService : IContractService
    {
        private static List<Contract> _contracts;
        private static readonly string _filePath = "./../SponsorshipManagement.Infrastructure/Data/contracts.json";
        private readonly IEventService? _eventService;
        private readonly IContractRepository? _contractRepository;

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
            return Task.FromResult(_contracts.Select(MapToDto));
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
            return Task.FromResult(MapToDto(contract));
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
            return Task.FromResult<ContractDto?>(MapToDto(contract));
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

        public ContractService(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
            _eventService = null;
        }

        public async Task<ContractDto?> GetContractByIdAsync(Guid id)
        {
            if (_contractRepository == null)
                throw new InvalidOperationException("_contractRepository is not initialized.");
            var contract = await _contractRepository.GetByIdAsync(id);
            return contract != null ? MapToDto(contract) : null;
        }

        public async Task<IEnumerable<ContractDto>> GetContractsBySponsorIdAsync(string sponsorId)
        {
            if (_contractRepository == null)
                throw new InvalidOperationException("_contractRepository is not initialized.");
            var contracts = await _contractRepository.GetBySponsorIdAsync(sponsorId);
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetContractsByEventIdAsync(string eventId)
        {
            if (_contractRepository == null)
                throw new InvalidOperationException("_contractRepository is not initialized.");
            var contracts = await _contractRepository.GetByEventIdAsync(eventId);
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetActiveContractsAsync()
        {
            if (_contractRepository == null)
                throw new InvalidOperationException("_contractRepository is not initialized.");
            var contracts = await _contractRepository.GetActiveContractsAsync();
            return contracts.Select(MapToDto);
        }

        public async Task<ContractDto> UpdateContractAsync(ContractDto contractDto)
        {
            if (_contractRepository == null)
                throw new InvalidOperationException("_contractRepository is not initialized.");
            var existingContract = await _contractRepository.GetByIdAsync(contractDto.Id);
            if (existingContract == null)
                throw new ArgumentException($"Contrato con ID {contractDto.Id} no encontrado");

            existingContract.Title = contractDto.Title;
            existingContract.Description = contractDto.Description;
            existingContract.Value = contractDto.Value;
            existingContract.StartDate = contractDto.StartDate;
            existingContract.EndDate = contractDto.EndDate;
            existingContract.SponsorId = contractDto.SponsorId;
            existingContract.EventId = contractDto.EventId;
            
            if (Enum.TryParse<ContractStatus>(contractDto.Status.ToString(), out var status))
                existingContract.Status = status;

            var updatedContract = await _contractRepository.UpdateAsync(existingContract);
            return MapToDto(updatedContract);
        }

        public async Task<bool> DeleteContractAsync(Guid id)
        {
            if (_contractRepository == null)
                throw new InvalidOperationException("_contractRepository is not initialized.");
            return await _contractRepository.DeleteAsync(id);
        }

        public async Task<bool> ContractExistsAsync(Guid id)
        {
            if (_contractRepository == null)
                throw new InvalidOperationException("_contractRepository is not initialized.");
            var contract = await _contractRepository.GetByIdAsync(id);
            return contract != null;
        }

        private ContractDto MapToDto(Contract contract)
        {
            return new ContractDto
            {
                Id = contract.Id,
                Title = contract.Title,
                Description = contract.Description,
                Value = contract.Value,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = (int)contract.Status,
                SponsorId = contract.SponsorId,
                EventId = contract.EventId,
                CreatedAt = contract.CreatedAt,
                UpdatedAt = contract.UpdatedAt,
                IsActive = contract.IsActive()
            };
        }
        private static Contract ToEntity(ContractDto dto)
        {
            return new Contract
            {
                Id = dto.Id != Guid.Empty ? dto.Id : Guid.NewGuid(),
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
            var eventDto = _eventService?.GetEventById(eventId);
            if (eventDto != null)
            {
                eventDto.Fund = total;
                _eventService?.UpdateEvent(eventDto);
            }
        }
    }
}