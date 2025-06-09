using SponsorshipManagement.Application.Dtos;
using System.Collections.Generic;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface IEventService
    {
        IEnumerable<EventDto> ListEvents();
        IEnumerable<EventDto> FilterEvents(string place = null!, decimal? fundMin = null, int? capacityMin = null);
    }
}
