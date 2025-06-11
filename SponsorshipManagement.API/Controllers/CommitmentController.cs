using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de compromisos contractuales
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CommitmentController : ControllerBase
    {
        private readonly ICommitmentService _commitmentService;

        public CommitmentController(ICommitmentService commitmentService)
        {
            _commitmentService = commitmentService;
        }

        /// <summary>
        /// Registra un nuevo compromiso contractual
        /// </summary>
        /// <param name="request">Datos del compromiso a crear</param>
        /// <returns>Compromiso creado</returns>
        /// <response code="201">Compromiso creado exitosamente</response>
        /// <response code="400">Datos inválidos o faltantes</response>
        /// <response code="409">El contrato especificado no existe</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(CommitmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CommitmentDto>> RegisterCommitment([FromBody] CreateCommitmentRequest request)
        {
            try
            {
                // Log de entrada para debugging
                // Console.WriteLine($"📥 Recibiendo request para crear compromiso:");
                // Console.WriteLine($"   ContractId: {request?.ContractId}");
                // Console.WriteLine($"   Description: {request?.Description}");
                // Console.WriteLine($"   DueDate: {request?.DueDate}");
                // Console.WriteLine($"   Responsible: {request?.Responsible}");

                // Validación de request nulo
                if (request == null)
                {
                    return BadRequest("Los datos del compromiso son requeridos");
                }

                // Crear el compromiso usando el servicio
                var commitment = await _commitmentService.CreateCommitmentAsync(request);
                
                if (commitment == null)
                {
                    return StatusCode(500, "Error interno al crear el compromiso");
                }

                // Console.WriteLine($"✅ Compromiso creado exitosamente: {commitment.Id}");

                // Retornar respuesta 201 Created con el recurso creado
                return CreatedAtAction(
                    nameof(GetCommitmentById), 
                    new { id = commitment.Id }, 
                    commitment
                );
            }
            catch (ArgumentException ex)
            {
                // A2: Faltan datos obligatorios → Error de validación
                // Console.WriteLine($"❌ Error de validación: {ex.Message}");
                return BadRequest($"Error de validación: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                // A1: El contrato no existe → Conflicto
                // Console.WriteLine($"❌ Error de negocio: {ex.Message}");
                return Conflict($"Error de negocio: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Error interno no controlado
                // Console.WriteLine($"❌ Error interno: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un compromiso por su ID
        /// </summary>
        /// <param name="id">ID del compromiso</param>
        /// <returns>Datos del compromiso</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommitmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CommitmentDto>> GetCommitmentById(Guid id)
        {
            try
            {
                var commitment = await _commitmentService.GetCommitmentByIdAsync(id);
                
                if (commitment == null)
                {
                    return NotFound($"Compromiso con ID {id} no encontrado");
                }

                return Ok(commitment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene todos los compromisos
        /// </summary>
        /// <returns>Lista de compromisos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CommitmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommitmentDto>>> GetAllCommitments()
        {
            try
            {
                var commitments = await _commitmentService.GetAllCommitmentsAsync();
                return Ok(commitments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene compromisos por ID de contrato
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>Lista de compromisos del contrato</returns>
        [HttpGet("contract/{contractId}")]
        [ProducesResponseType(typeof(IEnumerable<CommitmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CommitmentDto>>> GetCommitmentsByContract(Guid contractId)
        {
            try
            {
                var commitments = await _commitmentService.GetCommitmentsByContractIdAsync(contractId);
                return Ok(commitments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
