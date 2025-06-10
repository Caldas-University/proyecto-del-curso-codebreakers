using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Application.Dtos; // Añadir para SponsorDto
using SponsorshipManagement.API.Mappers; // Añadir para SponsorMapper

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

        // Nuevo endpoint para obtener un sponsor por número de documento
        [HttpGet("{documentNumber}")]
        public async Task<IActionResult> GetSponsorByDocument(string documentNumber)
        {
            var sponsorDto = await _sponsorService.GetSponsorByDocumentAsync(documentNumber);
            if (sponsorDto == null)
            {
                return NotFound();
            }
            return Ok(sponsorDto); // Devuelve el DTO directamente
        }

        // Ejemplo de un endpoint POST que usaría el mapper para convertir un DTO a Entidad
        [HttpPost]
        public IActionResult CreateSponsor([FromBody] SponsorDto sponsorDto) // Eliminado async Task<> ya que no hay await
        {
            if (sponsorDto == null)
            {
                return BadRequest();
            }

            // Si ISponsorService.CreateSponsorAsync esperara una entidad Sponsor, harías:
            // var sponsorEntity = SponsorMapper.ToEntity(sponsorDto);
            // await _sponsorService.CreateSponsorAsync(sponsorEntity); // Necesitarías que este método sea async y exista en el servicio

            // Este es un ejemplo conceptual, necesitarías implementar la lógica de creación en tu servicio.
            // Por ejemplo, podrías tener un método en ISponsorService que acepte SponsorDto:
            // var createdSponsorDto = await _sponsorService.CreateSponsorAsync(sponsorDto);
            // if(createdSponsorDto == null) { return BadRequest("No se pudo crear el sponsor."); }
            // return CreatedAtAction(nameof(GetSponsorByDocument), new { documentNumber = createdSponsorDto.DocumentNumber }, createdSponsorDto);

            // Para este ejemplo simplificado, devolvemos el DTO recibido.
            // En un caso real, llamarías al servicio para crear el recurso y luego devolverías el resultado.
            return CreatedAtAction(nameof(GetSponsorByDocument), new { documentNumber = sponsorDto.DocumentNumber }, sponsorDto);
        }

        [HttpGet("requirements-status/{documentNumber}")]
        public async Task<IActionResult> GetSponsorRequirementsStatus(string documentNumber)
        {
            var sponsorDto = await _sponsorService.GetSponsorByDocumentAsync(documentNumber);
            if (sponsorDto == null)
                return NotFound();
            var isValid = !string.IsNullOrEmpty(sponsorDto.Role) && sponsorDto.HasDocumentation;
            return Ok(new { isValid });
        }
    }
}
