using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;

namespace SponsorshipManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisingBenefitExecutionController : ControllerBase
    {
        private readonly IAdvertisingBenefitExecutionService _executionService;

        public AdvertisingBenefitExecutionController(IAdvertisingBenefitExecutionService executionService)
        {
            _executionService = executionService;
        }

        /// <summary>
        /// Obtiene todas las ejecuciones de beneficios publicitarios
        /// </summary>
        /// <returns>Lista de ejecuciones</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdvertisingBenefitExecutionDto>>> GetAll()
        {
            var executions = await _executionService.GetAllExecutionsAsync();
            return Ok(executions);
        }

        /// <summary>
        /// Obtiene una ejecución por ID
        /// </summary>
        /// <param name="id">ID de la ejecución</param>
        /// <returns>Ejecución de beneficio</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AdvertisingBenefitExecutionDto>> GetById(Guid id)
        {
            var execution = await _executionService.GetExecutionByIdAsync(id);
            if (execution == null)
                return NotFound($"Ejecución con ID {id} no encontrada");
            
            return Ok(execution);
        }

        /// <summary>
        /// Obtiene ejecuciones por patrocinador
        /// </summary>
        /// <param name="sponsorDocument">Número de documento del patrocinador</param>
        /// <returns>Lista de ejecuciones del patrocinador</returns>
        [HttpGet("sponsor/{sponsorDocument}")]
        public async Task<ActionResult<IEnumerable<AdvertisingBenefitExecutionDto>>> GetBySponsor(string sponsorDocument)
        {
            var executions = await _executionService.GetExecutionsBySponsorAsync(sponsorDocument);
            return Ok(executions);
        }

        /// <summary>
        /// Obtiene ejecuciones por evento
        /// </summary>
        /// <param name="eventId">ID del evento</param>
        /// <returns>Lista de ejecuciones del evento</returns>
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<AdvertisingBenefitExecutionDto>>> GetByEvent(string eventId)
        {
            var executions = await _executionService.GetExecutionsByEventAsync(eventId);
            return Ok(executions);
        }

        /// <summary>
        /// Obtiene ejecuciones por contrato
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Lista de ejecuciones del contrato</returns>
        [HttpGet("contract/{contractId}")]
        public async Task<ActionResult<IEnumerable<AdvertisingBenefitExecutionDto>>> GetByContract(Guid contractId)
        {
            var executions = await _executionService.GetExecutionsByContractAsync(contractId);
            return Ok(executions);
        }

        /// <summary>
        /// Obtiene ejecuciones por estado
        /// </summary>
        /// <param name="status">Estado de la ejecución (Pendiente, Ejecutado, Cancelado)</param>
        /// <returns>Lista de ejecuciones con el estado especificado</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<AdvertisingBenefitExecutionDto>>> GetByStatus(string status)
        {
            var executions = await _executionService.GetExecutionsByStatusAsync(status);
            return Ok(executions);
        }

        /// <summary>
        /// Crea una nueva ejecución de beneficio
        /// </summary>
        /// <param name="executionDto">Datos de la ejecución a crear</param>
        /// <returns>Ejecución creada</returns>
        [HttpPost]
        public async Task<ActionResult<AdvertisingBenefitExecutionDto>> Create([FromBody] AdvertisingBenefitExecutionDto executionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdExecution = await _executionService.CreateExecutionAsync(executionDto);
            return CreatedAtAction(nameof(GetById), new { id = createdExecution.Id }, createdExecution);
        }

        /// <summary>
        /// Actualiza una ejecución existente
        /// </summary>
        /// <param name="id">ID de la ejecución a actualizar</param>
        /// <param name="executionDto">Datos actualizados de la ejecución</param>
        /// <returns>Ejecución actualizada</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<AdvertisingBenefitExecutionDto>> Update(Guid id, [FromBody] AdvertisingBenefitExecutionDto executionDto)
        {
            if (id != executionDto.Id)
                return BadRequest("El ID del parámetro no coincide con el ID de la ejecución");

            if (!await _executionService.ExecutionExistsAsync(id))
                return NotFound($"Ejecución con ID {id} no encontrada");

            var updatedExecution = await _executionService.UpdateExecutionAsync(executionDto);
            return Ok(updatedExecution);
        }

        /// <summary>
        /// Marca una ejecución como completada
        /// </summary>
        /// <param name="id">ID de la ejecución</param>
        /// <param name="completedBy">Usuario que completó la ejecución</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPatch("{id}/complete")]
        public async Task<ActionResult> MarkAsCompleted(Guid id, [FromQuery] string completedBy)
        {
            if (!await _executionService.ExecutionExistsAsync(id))
                return NotFound($"Ejecución con ID {id} no encontrada");

            var completed = await _executionService.MarkExecutionAsCompletedAsync(id, completedBy);
            if (completed)
                return NoContent();
            
            return BadRequest("No se pudo marcar la ejecución como completada");
        }

        /// <summary>
        /// Elimina una ejecución
        /// </summary>
        /// <param name="id">ID de la ejecución a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            if (!await _executionService.ExecutionExistsAsync(id))
                return NotFound($"Ejecución con ID {id} no encontrada");

            var deleted = await _executionService.DeleteExecutionAsync(id);
            if (deleted)
                return NoContent();
            
            return BadRequest("No se pudo eliminar la ejecución");
        }
    }
}