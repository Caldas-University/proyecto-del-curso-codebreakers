using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private static List<Event> _events;
        private static readonly string _jsonFilePath = "./../SponsorshipManagement.Infrastructure/Data/events.json";

        static EventRepository()
        {
            if (File.Exists(_jsonFilePath))
            {
                var json = File.ReadAllText(_jsonFilePath);
                _events = JsonSerializer.Deserialize<List<Event>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Event>();
            }
            else
            {
                _events = new List<Event>();
            }
        }

        public IEnumerable<Event> ListEvents()
        {
            return _events;
        }

        public IEnumerable<Event> FilterEvents(string place = null!, decimal? fundMin = null, int? capacityMin = null)
        {
            return _events.Where(e =>
                (string.IsNullOrEmpty(place) || e.Place == place) &&
                (!fundMin.HasValue || e.Fund >= fundMin) &&
                (!capacityMin.HasValue || e.Capacity >= capacityMin)
            );
        }

        public Event? GetEventById(string id)
        {
            return _events.FirstOrDefault(e => e.Id.ToString() == id);
        }

        public void UpdateEvent(Event updatedEvent)
        {
            var idx = _events.FindIndex(e => e.Id == updatedEvent.Id);
            if (idx >= 0)
            {
                _events[idx] = updatedEvent;
                SaveChanges();
            }
        }

        private static void SaveChanges()
        {
            var json = JsonSerializer.Serialize(_events, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_jsonFilePath, json);
        }
    }
}
