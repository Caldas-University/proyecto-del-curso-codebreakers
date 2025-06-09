using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.API.Mappers
{
    public static class EventMapper
    {
        public static EventDto ToDto(Event ev)
        {
            return new EventDto
            {
                Id = ev.Id.ToString(),
                Place = ev.Place,
                Fund = ev.Fund,
                Capacity = ev.Capacity,
                Sponsors = ev.Sponsors
            };
        }

        public static Event ToEntity(EventDto dto)
        {
            return new Event
            {
                Id = Guid.TryParse(dto.Id, out var guid) ? guid : Guid.Empty,
                Place = dto.Place,
                Fund = dto.Fund,
                Capacity = dto.Capacity,
                Sponsors = dto.Sponsors
            };
        }
    }
}
