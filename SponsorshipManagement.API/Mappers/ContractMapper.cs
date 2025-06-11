using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Application.Dtos;
using System.Text.Json;

namespace SponsorshipManagement.API.Mappers
{
    public static class ContractMapper
    {
        public static ContractDto ToDto(Contract contract)
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

        public static Contract ToEntity(ContractDto dto)
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
                CreatedAt = dto.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = dto.UpdatedAt
            };
        }
    }
}