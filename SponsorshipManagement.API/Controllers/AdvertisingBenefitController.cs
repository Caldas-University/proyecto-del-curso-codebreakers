using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;

namespace SponsorshipManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisingBenefitController : ControllerBase
    {
        private readonly IAdvertisingBenefitService _benefitService;

        public AdvertisingBenefitController(IAdvertisingBenefitService benefitService)
        {
            _benefitService = benefitService;
        }

        /// <summary>
        /// Obtiene todos los beneficios publicitarios
        /// </summary>
        /// <returns>Lista de beneficios publicitarios</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdvertisingBenefitDto>>> GetAll()
        {
            var benefits = await _benefitService.GetAllBenefitsAsync();
            return Ok(benefits);
        }

        /// <summary>
        /// Obtiene un beneficio publicitario por ID
        /// </summary>
        /// <param name="id">ID del beneficio</param>
        /// <returns>Beneficio publicitario</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AdvertisingBenefitDto>> GetById(Guid id)
        {
            var benefit = await _benefitService.GetBenefitByIdAsync(id);
            if (benefit == null)
                return NotFound($"Beneficio con ID {id} no encontrado");
            
            return Ok(benefit);
        }

        /// <summary>
        /// Obtiene beneficios publicitarios por tipo
        /// </summary>
        /// <param name="type">Tipo de beneficio (Logo, Banner, Mencion, Stand, etc.)</param>
        /// <returns>Lista de beneficios del tipo especificado</returns>
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<AdvertisingBenefitDto>>> GetByType(string type)
        {
            var benefits = await _benefitService.GetBenefitsByTypeAsync(type);
            return Ok(benefits);
        }

        /// <summary>
        /// Crea un nuevo beneficio publicitario
        /// </summary>
        /// <param name="benefitDto">Datos del beneficio a crear</param>
        /// <returns>Beneficio creado</returns>
        [HttpPost]
        public async Task<ActionResult<AdvertisingBenefitDto>> Create([FromBody] AdvertisingBenefitDto benefitDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBenefit = await _benefitService.CreateBenefitAsync(benefitDto);
            return CreatedAtAction(nameof(GetById), new { id = createdBenefit.Id }, createdBenefit);
        }

        /// <summary>
        /// Actualiza un beneficio publicitario existente
        /// </summary>
        /// <param name="id">ID del beneficio a actualizar</param>
        /// <param name="benefitDto">Datos actualizados del beneficio</param>
        /// <returns>Beneficio actualizado</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<AdvertisingBenefitDto>> Update(Guid id, [FromBody] AdvertisingBenefitDto benefitDto)
        {
            if (id != benefitDto.Id)
                return BadRequest("El ID del parámetro no coincide con el ID del beneficio");

            if (!await _benefitService.BenefitExistsAsync(id))
                return NotFound($"Beneficio con ID {id} no encontrado");

            var updatedBenefit = await _benefitService.UpdateBenefitAsync(benefitDto);
            return Ok(updatedBenefit);
        }

        /// <summary>
        /// Elimina un beneficio publicitario
        /// </summary>
        /// <param name="id">ID del beneficio a eliminar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            if (!await _benefitService.BenefitExistsAsync(id))
                return NotFound($"Beneficio con ID {id} no encontrado");

            var deleted = await _benefitService.DeleteBenefitAsync(id);
            if (deleted)
                return NoContent();
            
            return BadRequest("No se pudo eliminar el beneficio");
        }
    }
}