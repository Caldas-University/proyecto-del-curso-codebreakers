using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Interfaces;

namespace SponsorshipManagement.API.Controllers
{
    [ApiController]
    [Route("api/sponsors")]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;

        public SponsorController(ISponsorService sponsorService)
        {
            _sponsorService = sponsorService;
        }

        [HttpGet("exists/{documentNumber}")]
        public async Task<IActionResult> CheckSponsorExists(string documentNumber)
        {
            var exists = await _sponsorService.SponsorExistsAsync(documentNumber);
            return Ok(new { exists });
        }
    }
}
