using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.API.Mappers
{
    public static class CommitmentMapper
    {
        public static CommitmentDto ToDto(Commitment commitment)
        {
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

        public static Commitment ToEntity(CommitmentDto dto)
        {
            var commitment = new Commitment();
            
            if (Guid.TryParse(dto.Id, out var id))
                commitment.Id = id;
            
            if (Guid.TryParse(dto.ContractId, out var contractId))
                commitment.ContractId = contractId;
            
            commitment.Description = dto.Description;
            commitment.Obligations = dto.Obligations;
            commitment.DueDate = dto.DueDate;
            commitment.Responsible = dto.Responsible;
            
            if (Enum.TryParse<CommitmentStatus>(dto.Status, out var status))
                commitment.Status = status;
            
            commitment.CreatedAt = dto.CreatedAt;
            commitment.UpdatedAt = dto.UpdatedAt;

            return commitment;
        }

        public static Commitment ToEntity(CreateCommitmentRequest request)
        {
            if (!Guid.TryParse(request.ContractId, out var contractId))
                throw new ArgumentException("Invalid ContractId format");

            return new Commitment(
                contractId,
                request.Description,
                request.Obligations,
                request.DueDate,
                request.Responsible
            );
        }
    }
}
