using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;

namespace SponsorshipManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricController : ControllerBase
    {
        private readonly IMetricService _metricService;

        public MetricController(IMetricService metricService)
        {
            _metricService = metricService;
        }

        /// <summary>
        /// Obtiene todas las métricas disponibles
        /// </summary>
        /// <returns>Lista de métricas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetricDto>>> GetAll()
        {
            var metrics = await _metricService.GetAllMetricsAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Obtiene una métrica por ID
        /// </summary>
        /// <param name="id">ID de la métrica</param>
        /// <returns>Métrica</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MetricDto>> GetById(Guid id)
        {
            var metric = await _metricService.GetMetricByIdAsync(id);
            if (metric == null)
                return NotFound($"Métrica con ID {id} no encontrada");
            
            return Ok(metric);
        }

        /// <summary>
        /// Obtiene métricas por tipo
        /// </summary>
        /// <param name="type">Tipo de métrica (Alcance, Interaccion, Comparativa)</param>
        /// <returns>Lista de métricas del tipo especificado</returns>
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<MetricDto>>> GetByType(string type)
        {
            var metrics = await _metricService.GetMetricsByTypeAsync(type);
            return Ok(metrics);
        }

        /// <summary>
        /// Obtiene métricas por categoría
        /// </summary>
        /// <param name="category">Categoría de métrica (Social, Web, Evento, General)</param>
        /// <returns>Lista de métricas de la categoría especificada</returns>
        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<MetricDto>>> GetByCategory(string category)
        {
            var metrics = await _metricService.GetMetricsByCategoryAsync(category);
            return Ok(metrics);
        }

        /// <summary>
        /// Crea una nueva métrica
        /// </summary>
        /// <param name="metricDto">Datos de la métrica a crear</param>
        /// <returns>Métrica creada</returns>
        [HttpPost]
        public async Task<ActionResult<MetricDto>> Create([FromBody] MetricDto metricDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdMetric = await _metricService.CreateMetricAsync(metricDto);
            return CreatedAtAction(nameof(GetById), new { id = createdMetric.Id }, createdMetric);
        }

        /// <summary>
        /// Actualiza una métrica existente
        /// </summary>
        /// <param name="id">ID de la métrica a actualizar</param>
        /// <param name="metricDto">Datos actualizados de la métrica</param>
        /// <returns>Métrica actualizada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<MetricDto>> Update(Guid id, [FromBody] MetricDto metricDto)
        {
            if (id != metricDto.Id)
                return BadRequest("El ID del parámetro no coincide con el ID de la métrica");

            if (!await _metricService.MetricExistsAsync(id))
                return NotFound($"Métrica con ID {id} no encontrada");

            var updatedMetric = await _metricService.UpdateMetricAsync(metricDto);
            return Ok(updatedMetric);
        }

        /// <summary>
        /// Elimina una métrica
        /// </summary>
        /// <param name="id">ID de la métrica a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            if (!await _metricService.MetricExistsAsync(id))
                return NotFound($"Métrica con ID {id} no encontrada");

            var deleted = await _metricService.DeleteMetricAsync(id);
            if (deleted)
                return NoContent();
            
            return BadRequest("No se pudo eliminar la métrica");
        }
    }
}