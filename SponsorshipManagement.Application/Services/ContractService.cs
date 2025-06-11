using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;

        public ContractService(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<IEnumerable<ContractDto>> GetAllContractsAsync()
        {
            var contracts = await _contractRepository.GetAllAsync();
            return contracts.Select(MapToDto);
        }

        public async Task<ContractDto?> GetContractByIdAsync(Guid id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            return contract != null ? MapToDto(contract) : null;
        }

        public async Task<IEnumerable<ContractDto>> GetContractsBySponsorIdAsync(string sponsorId)
        {
            var contracts = await _contractRepository.GetBySponsorIdAsync(sponsorId);
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetContractsByEventIdAsync(string eventId)
        {
            var contracts = await _contractRepository.GetByEventIdAsync(eventId);
            return contracts.Select(MapToDto);
        }

        public async Task<IEnumerable<ContractDto>> GetActiveContractsAsync()
        {
            var contracts = await _contractRepository.GetActiveContractsAsync();
            return contracts.Select(MapToDto);
        }

        public async Task<ContractDto> CreateContractAsync(ContractDto contractDto)
        {
            var contract = new Contract(
                contractDto.Title,
                contractDto.Description,
                contractDto.Value,
                contractDto.StartDate,
                contractDto.EndDate,
                contractDto.SponsorId,
                contractDto.EventId
            );

            var createdContract = await _contractRepository.CreateAsync(contract);
            return MapToDto(createdContract);
        }

        public async Task<ContractDto> UpdateContractAsync(ContractDto contractDto)
        {
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
            
            if (Enum.TryParse<ContractStatus>(contractDto.Status, out var status))
                existingContract.Status = status;

            var updatedContract = await _contractRepository.UpdateAsync(existingContract);
            return MapToDto(updatedContract);
        }

        public async Task<bool> DeleteContractAsync(Guid id)
        {
            return await _contractRepository.DeleteAsync(id);
        }

        public async Task<bool> ContractExistsAsync(Guid id)
        {
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
                Status = contract.Status.ToString(),
                SponsorId = contract.SponsorId,
                EventId = contract.EventId,
                CreatedAt = contract.CreatedAt,
                UpdatedAt = contract.UpdatedAt,
                IsActive = contract.IsActive()
            };
        }
    }
}