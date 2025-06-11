using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;

namespace SponsorshipManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricComparisonController : ControllerBase
    {
        private readonly IMetricComparisonService _comparisonService;

        public MetricComparisonController(IMetricComparisonService comparisonService)
        {
            _comparisonService = comparisonService;
        }

        /// <summary>
        /// Obtiene todas las comparaciones de métricas
        /// </summary>
        /// <returns>Lista de comparaciones</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetricComparisonDto>>> GetAll()
        {
            var comparisons = await _comparisonService.GetAllComparisonsAsync();
            return Ok(comparisons);
        }

        /// <summary>
        /// Obtiene una comparación por ID
        /// </summary>
        /// <param name="id">ID de la comparación</param>
        /// <returns>Comparación de métricas</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MetricComparisonDto>> GetById(Guid id)
        {
            var comparison = await _comparisonService.GetComparisonByIdAsync(id);
            if (comparison == null)
                return NotFound($"Comparación con ID {id} no encontrada");
            
            return Ok(comparison);
        }

        /// <summary>
        /// Obtiene comparaciones por tipo
        /// </summary>
        /// <param name="type">Tipo de comparación (PeriodOverPeriod, EventComparison, SponsorComparison)</param>
        /// <returns>Lista de comparaciones del tipo especificado</returns>
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<MetricComparisonDto>>> GetByType(string type)
        {
            var comparisons = await _comparisonService.GetComparisonsByTypeAsync(type);
            return Ok(comparisons);
        }

        /// <summary>
        /// Crea una nueva comparación de métricas
        /// </summary>
        /// <param name="comparisonDto">Datos de la comparación a crear</param>
        /// <returns>Comparación creada</returns>
        [HttpPost]
        public async Task<ActionResult<MetricComparisonDto>> Create([FromBody] MetricComparisonDto comparisonDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdComparison = await _comparisonService.CreateComparisonAsync(comparisonDto);
            return CreatedAtAction(nameof(GetById), new { id = createdComparison.Id }, createdComparison);
        }

        /// <summary>
        /// Genera comparación automática entre períodos
        /// </summary>
        /// <param name="request">Parámetros para la comparación de períodos</param>
        /// <returns>Comparación generada</returns>
        [HttpPost("generate/period-comparison")]
        public async Task<ActionResult<MetricComparisonDto>> GeneratePeriodComparison([FromBody] PeriodComparisonRequest request)
        {
            var comparison = await _comparisonService.GeneratePeriodComparisonAsync(
                request.MetricIds, 
                request.Period1Start, 
                request.Period1End, 
                request.Period2Start, 
                request.Period2End
            );
            return Ok(comparison);
        }

        /// <summary>
        /// Genera comparación automática entre eventos
        /// </summary>
        /// <param name="request">Parámetros para la comparación de eventos</param>
        /// <returns>Comparación generada</returns>
        [HttpPost("generate/event-comparison")]
        public async Task<ActionResult<MetricComparisonDto>> GenerateEventComparison([FromBody] EventComparisonRequest request)
        {
            var comparison = await _comparisonService.GenerateEventComparisonAsync(request.MetricIds, request.Event1Id, request.Event2Id);
            return Ok(comparison);
        }

        /// <summary>
        /// Genera comparación automática entre patrocinadores
        /// </summary>
        /// <param name="request">Parámetros para la comparación de patrocinadores</param>
        /// <returns>Comparación generada</returns>
        [HttpPost("generate/sponsor-comparison")]
        public async Task<ActionResult<MetricComparisonDto>> GenerateSponsorComparison([FromBody] SponsorComparisonRequest request)
        {
            var comparison = await _comparisonService.GenerateSponsorComparisonAsync(
                request.MetricIds, 
                request.SponsorDocuments, 
                request.StartDate, 
                request.EndDate
            );
            return Ok(comparison);
        }

        /// <summary>
        /// Elimina una comparación
        /// </summary>
        /// <param name="id">ID de la comparación a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            if (!await _comparisonService.ComparisonExistsAsync(id))
                return NotFound($"Comparación con ID {id} no encontrada");

            var deleted = await _comparisonService.DeleteComparisonAsync(id);
            if (deleted)
                return NoContent();
            
            return BadRequest("No se pudo eliminar la comparación");
        }
    }

    // DTOs para las requests de comparación
    public class PeriodComparisonRequest
    {
        public List<Guid> MetricIds { get; set; } = new List<Guid>();
        public DateTime Period1Start { get; set; }
        public DateTime Period1End { get; set; }
        public DateTime Period2Start { get; set; }
        public DateTime Period2End { get; set; }
    }

    public class EventComparisonRequest
    {
        public List<Guid> MetricIds { get; set; } = new List<Guid>();
        public string Event1Id { get; set; } = string.Empty;
        public string Event2Id { get; set; } = string.Empty;
    }

    public class SponsorComparisonRequest
    {
        public List<Guid> MetricIds { get; set; } = new List<Guid>();
        public List<string> SponsorDocuments { get; set; } = new List<string>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}