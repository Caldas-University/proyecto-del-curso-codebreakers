using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de renovaciones y finalización de contratos
    /// 🎯 CU-PA-05: Gestionar renovaciones y finalización de contratos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ContractRenewalController : ControllerBase
    {
        private readonly IContractRenewalService _renewalService;

        public ContractRenewalController(IContractRenewalService renewalService)
        {
            _renewalService = renewalService;
        }

        /// <summary>
        /// Obtiene contratos que vencen en los próximos N días
        /// 🎯 CU-PA-05.01.1: Consulta de contratos próximos a vencer
        /// </summary>
        /// <param name="days">Número de días para considerar (por defecto 30)</param>
        /// <param name="includeStatuses">Estados de contrato a incluir (opcional)</param>
        /// <returns>Lista de contratos próximos a vencer</returns>
        [HttpGet("expiring")]
        public async Task<ActionResult<IEnumerable<ContractRenewalDto>>> GetExpiringContracts(
            [FromQuery] int days = 30,
            [FromQuery] string[]? includeStatuses = null)
        {
            try
            {
                Console.WriteLine($"📥 Endpoint /api/ContractRenewal/expiring invocado con days={days}");
                
                if (includeStatuses == null || includeStatuses.Length == 0)
                {
                    includeStatuses = new[] { "Active", "Signed" };
                }
                
                Console.WriteLine($"🔍 Buscando contratos con estados: {string.Join(", ", includeStatuses)}");
                
                var contracts = await _renewalService.GetContractsExpiringInDaysAsync(days, includeStatuses);
                
                Console.WriteLine($"✅ Encontrados {contracts.Count()} contratos próximos a vencer");
                
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR EN ENDPOINT: {ex.Message}");
                Console.WriteLine($"❌ STACK TRACE: {ex.StackTrace}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Marca un contrato como notificado para renovación
        /// </summary>
        /// <param name="id">ID del contrato</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("{id}/notify")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> MarkAsNotified(Guid id)
        {
            try
            {
                var success = await _renewalService.MarkContractAsNotifiedAsync(id);
                
                if (!success)
                    return NotFound($"Contrato con ID {id} no encontrado");
                
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error marcando contrato como notificado: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}