using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace SponsorshipManagement.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public IEnumerable<EventDto> ListEvents()
        {
            return _eventRepository.ListEvents().Select(e => new EventDto {
                Id = e.Id.ToString(),
                Place = e.Place,
                Fund = e.Fund,
                Capacity = e.Capacity,
                Sponsors = e.Sponsors
            });
        }

        public IEnumerable<EventDto> FilterEvents(string place = null!, decimal? fundMin = null, int? capacityMin = null)
        {
            return _eventRepository.FilterEvents(place, fundMin, capacityMin).Select(e => new EventDto {
                Id = e.Id.ToString(),
                Place = e.Place,
                Fund = e.Fund,
                Capacity = e.Capacity,
                Sponsors = e.Sponsors
            });
        }
    }
}
