using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;

namespace SponsorshipManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricRecordController : ControllerBase
    {
        private readonly IMetricRecordService _recordService;

        public MetricRecordController(IMetricRecordService recordService)
        {
            _recordService = recordService;
        }

        /// <summary>
        /// Obtiene todos los registros de métricas
        /// </summary>
        /// <returns>Lista de registros de métricas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetricRecordDto>>> GetAll()
        {
            var records = await _recordService.GetAllRecordsAsync();
            return Ok(records);
        }

        /// <summary>
        /// Obtiene un registro de métrica por ID
        /// </summary>
        /// <param name="id">ID del registro</param>
        /// <returns>Registro de métrica</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<MetricRecordDto>> GetById(Guid id)
        {
            var record = await _recordService.GetRecordByIdAsync(id);
            if (record == null)
                return NotFound($"Registro con ID {id} no encontrado");
            
            return Ok(record);
        }

        /// <summary>
        /// Obtiene registros por métrica
        /// </summary>
        /// <param name="metricId">ID de la métrica</param>
        /// <returns>Lista de registros de la métrica</returns>
        [HttpGet("metric/{metricId}")]
        public async Task<ActionResult<IEnumerable<MetricRecordDto>>> GetByMetric(Guid metricId)
        {
            var records = await _recordService.GetRecordsByMetricAsync(metricId);
            return Ok(records);
        }

        /// <summary>
        /// Obtiene registros por ejecución de beneficio
        /// </summary>
        /// <param name="executionId">ID de la ejecución</param>
        /// <returns>Lista de registros de la ejecución</returns>
        [HttpGet("execution/{executionId}")]
        public async Task<ActionResult<IEnumerable<MetricRecordDto>>> GetByExecution(Guid executionId)
        {
            var records = await _recordService.GetRecordsByExecutionAsync(executionId);
            return Ok(records);
        }

        /// <summary>
        /// Obtiene registros por patrocinador
        /// </summary>
        /// <param name="sponsorDocument">Número de documento del patrocinador</param>
        /// <returns>Lista de registros del patrocinador</returns>
        [HttpGet("sponsor/{sponsorDocument}")]
        public async Task<ActionResult<IEnumerable<MetricRecordDto>>> GetBySponsor(string sponsorDocument)
        {
            var records = await _recordService.GetRecordsBySponsorAsync(sponsorDocument);
            return Ok(records);
        }

        /// <summary>
        /// Obtiene registros por evento
        /// </summary>
        /// <param name="eventId">ID del evento</param>
        /// <returns>Lista de registros del evento</returns>
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<MetricRecordDto>>> GetByEvent(string eventId)
        {
            var records = await _recordService.GetRecordsByEventAsync(eventId);
            return Ok(records);
        }

        /// <summary>
        /// Obtiene registros por período de tiempo
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Lista de registros en el período</returns>
        [HttpGet("period")]
        public async Task<ActionResult<IEnumerable<MetricRecordDto>>> GetByPeriod([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var records = await _recordService.GetRecordsByPeriodAsync(startDate, endDate);
            return Ok(records);
        }

        /// <summary>
        /// Crea un nuevo registro de métrica
        /// </summary>
        /// <param name="recordDto">Datos del registro a crear</param>
        /// <returns>Registro creado</returns>
        [HttpPost]
        public async Task<ActionResult<MetricRecordDto>> Create([FromBody] MetricRecordDto recordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdRecord = await _recordService.CreateRecordAsync(recordDto);
            return CreatedAtAction(nameof(GetById), new { id = createdRecord.Id }, createdRecord);
        }

        /// <summary>
        /// Actualiza un registro existente
        /// </summary>
        /// <param name="id">ID del registro a actualizar</param>
        /// <param name="recordDto">Datos actualizados del registro</param>
        /// <returns>Registro actualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<MetricRecordDto>> Update(Guid id, [FromBody] MetricRecordDto recordDto)
        {
            if (id != recordDto.Id)
                return BadRequest("El ID del parámetro no coincide con el ID del registro");

            if (!await _recordService.RecordExistsAsync(id))
                return NotFound($"Registro con ID {id} no encontrado");

            var updatedRecord = await _recordService.UpdateRecordAsync(recordDto);
            return Ok(updatedRecord);
        }

        /// <summary>
        /// Elimina un registro
        /// </summary>
        /// <param name="id">ID del registro a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            if (!await _recordService.RecordExistsAsync(id))
                return NotFound($"Registro con ID {id} no encontrado");

            var deleted = await _recordService.DeleteRecordAsync(id);
            if (deleted)
                return NoContent();
            
            return BadRequest("No se pudo eliminar el registro");
        }

        /// <summary>
        /// Calcula el promedio de una métrica en un período
        /// </summary>
        /// <param name="metricId">ID de la métrica</param>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Promedio calculado</returns>
        [HttpGet("metric/{metricId}/average")]
        public async Task<ActionResult<decimal>> CalculateAverage(Guid metricId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var average = await _recordService.CalculateAverageAsync(metricId, startDate, endDate);
            return Ok(new { MetricId = metricId, Average = average, StartDate = startDate, EndDate = endDate });
        }

        /// <summary>
        /// Calcula el total de una métrica en un período
        /// </summary>
        /// <param name="metricId">ID de la métrica</param>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Total calculado</returns>
        [HttpGet("metric/{metricId}/total")]
        public async Task<ActionResult<decimal>> CalculateTotal(Guid metricId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var total = await _recordService.CalculateTotalAsync(metricId, startDate, endDate);
            return Ok(new { MetricId = metricId, Total = total, StartDate = startDate, EndDate = endDate });
        }
    }
}