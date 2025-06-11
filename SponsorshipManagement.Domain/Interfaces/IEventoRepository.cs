using SponsorshipManagement.Domain.Entities;
using System.Collections.Generic;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface IEventRepository
    {
        IEnumerable<Event> ListEvents();
        IEnumerable<Event> FilterEvents(string place = null!, decimal? fundMin = null, int? capacityMin = null);
        Event? GetEventById(string id);
        void UpdateEvent(Event updatedEvent);
    }
}
