using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class CommitmentService : ICommitmentService
    {
        private readonly ICommitmentRepository _commitmentRepository;

        public CommitmentService(ICommitmentRepository commitmentRepository)
        {
            _commitmentRepository = commitmentRepository;
        }

        public async Task<CommitmentDto?> GetCommitmentByIdAsync(Guid id)
        {
            var commitment = await _commitmentRepository.GetByIdAsync(id);
            if (commitment == null) return null;

            return new CommitmentDto
            {
                Id = commitment.Id.ToString(),
                ContractId = commitment.ContractId.ToString(),
                Description = commitment.Description,
                Obligations = commitment.Obligations,
                DueDate = commitment.DueDate,
                Responsible = commitment.Responsible,
                Status = commitment.Status.ToString(),
                CreatedAt = commitment.CreatedAt,
                UpdatedAt = commitment.UpdatedAt
            };
        }

        public async Task<IEnumerable<CommitmentDto>> GetCommitmentsByContractIdAsync(Guid contractId)
        {
            var commitments = await _commitmentRepository.GetByContractIdAsync(contractId);
            return commitments.Select(c => new CommitmentDto
            {
                Id = c.Id.ToString(),
                ContractId = c.ContractId.ToString(),
                Description = c.Description,
                Obligations = c.Obligations,
                DueDate = c.DueDate,
                Responsible = c.Responsible,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<IEnumerable<CommitmentDto>> GetAllCommitmentsAsync()
        {
            var commitments = await _commitmentRepository.GetAllAsync();
            return commitments.Select(c => new CommitmentDto
            {
                Id = c.Id.ToString(),
                ContractId = c.ContractId.ToString(),
                Description = c.Description,
                Obligations = c.Obligations,
                DueDate = c.DueDate,
                Responsible = c.Responsible,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<CommitmentDto?> CreateCommitmentAsync(CreateCommitmentRequest request)
        {
            // Validación de datos obligatorios
            if (string.IsNullOrWhiteSpace(request.ContractId) ||
                string.IsNullOrWhiteSpace(request.Description) ||
                string.IsNullOrWhiteSpace(request.Obligations) ||
                string.IsNullOrWhiteSpace(request.Responsible))
            {
                throw new ArgumentException("Faltan datos obligatorios");
            }

            if (!Guid.TryParse(request.ContractId, out var contractId))
            {
                throw new ArgumentException("ID de contrato inválido");
            }

            // Validación de existencia del contrato
            var contractExists = await _commitmentRepository.ContractExistsAsync(contractId);
            if (!contractExists)
            {
                throw new InvalidOperationException("El contrato no existe");
            }

            var commitment = new Commitment(
                contractId,
                request.Description,
                request.Obligations,
                request.DueDate,
                request.Responsible
            );

            var success = await _commitmentRepository.AddAsync(commitment);
            if (!success) return null;

            return new CommitmentDto
            {
                Id = commitment.Id.ToString(),
                ContractId = commitment.ContractId.ToString(),
                Description = commitment.Description,
                Obligations = commitment.Obligations,
                DueDate = commitment.DueDate,
                Responsible = commitment.Responsible,
                Status = commitment.Status.ToString(),
                CreatedAt = commitment.CreatedAt,
                UpdatedAt = commitment.UpdatedAt
            };
        }

        public async Task<CommitmentDto?> UpdateCommitmentAsync(Guid id, CreateCommitmentRequest request)
        {
            var existingCommitment = await _commitmentRepository.GetByIdAsync(id);
            if (existingCommitment == null) return null;

            // Validación de datos obligatorios
            if (string.IsNullOrWhiteSpace(request.Description) ||
                string.IsNullOrWhiteSpace(request.Obligations) ||
                string.IsNullOrWhiteSpace(request.Responsible))
            {
                throw new ArgumentException("Faltan datos obligatorios");
            }

            existingCommitment.UpdateCommitment(
                request.Description,
                request.Obligations,
                request.DueDate,
                request.Responsible
            );

            var success = await _commitmentRepository.UpdateAsync(existingCommitment);
            if (!success) return null;

            return new CommitmentDto
            {
                Id = existingCommitment.Id.ToString(),
                ContractId = existingCommitment.ContractId.ToString(),
                Description = existingCommitment.Description,
                Obligations = existingCommitment.Obligations,
                DueDate = existingCommitment.DueDate,
                Responsible = existingCommitment.Responsible,
                Status = existingCommitment.Status.ToString(),
                CreatedAt = existingCommitment.CreatedAt,
                UpdatedAt = existingCommitment.UpdatedAt
            };
        }

        public async Task<bool> DeleteCommitmentAsync(Guid id)
        {
            return await _commitmentRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<CommitmentDto>> GetOverdueCommitmentsAsync()
        {
            var commitments = await _commitmentRepository.GetOverdueCommitmentsAsync();
            return commitments.Select(c => new CommitmentDto
            {
                Id = c.Id.ToString(),
                ContractId = c.ContractId.ToString(),
                Description = c.Description,
                Obligations = c.Obligations,
                DueDate = c.DueDate,
                Responsible = c.Responsible,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }
    }
}