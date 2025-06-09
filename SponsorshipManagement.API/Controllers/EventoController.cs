using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Application.Dtos;
using System.Collections.Generic;

namespace SponsorshipManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("filter")]
        public ActionResult<IEnumerable<EventDto>> GetEvents([FromQuery] string place = null!, [FromQuery] decimal? fundMin = null, [FromQuery] int? capacityMin = null)
        {
            if (string.IsNullOrEmpty(place) && !fundMin.HasValue && !capacityMin.HasValue)
                return Ok(_eventService.ListEvents());
            return Ok(_eventService.FilterEvents(place, fundMin, capacityMin));
        }
    }
}
