using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Application.Dtos;
using System.Text.Json;

namespace SponsorshipManagement.API.Mappers
{
    public static class ContractHistoryMapper
    {
        public static ContractHistoryDto ToDto(ContractHistory history)
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

        public static ContractHistory ToEntity(ContractHistoryDto dto)
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
    }
}