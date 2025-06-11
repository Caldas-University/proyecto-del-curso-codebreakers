using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.API.Controllers
{
    [ApiController]
    [Route("api/contracts")]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IContractHistoryService _historyService;

        public ContractController(IContractService contractService, IContractHistoryService historyService)
        {
            _contractService = contractService;
            _historyService = historyService;
        }

        /// <summary>
        /// Get all contracts
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetAll()
        {
            var contracts = await _contractService.GetAllContractsAsync();
            return Ok(contracts);
        }

        /// <summary>
        /// Get contract by id
        /// </summary>
        [HttpGet("by-id/{id}")]
        public async Task<ActionResult<ContractDto>> GetById(string id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null) return NotFound();
            return Ok(contract);
        }

        /// <summary>
        /// Create a new contract
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<ContractDto>> Create([FromBody] ContractDto contractDto)
        {
            var created = await _contractService.CreateContractAsync(contractDto);
            await _historyService.AddHistoryAsync(new ContractHistoryDto {
                ContractId = created.Id,
                Action = "Created",
                PreviousVersionJson = string.Empty,
                Timestamp = created.CreatedAt ?? System.DateTime.UtcNow
            });
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update a contract
        /// </summary>
        [HttpPut("update/{id}")]
        public async Task<ActionResult<ContractDto>> Update(string id, [FromBody] ContractDto contractDto)
        {
            var previous = await _contractService.GetContractByIdAsync(id);
            if (previous == null) return NotFound();
            var previousJson = System.Text.Json.JsonSerializer.Serialize(previous);
            var updated = await _contractService.UpdateContractAsync(id, contractDto);
            await _historyService.AddHistoryAsync(new ContractHistoryDto {
                ContractId = id,
                Action = "Updated",
                PreviousVersionJson = previousJson,
                Timestamp = updated?.UpdatedAt ?? System.DateTime.UtcNow
            });
            return Ok(updated);
        }

        /// <summary>
        /// Delete a contract
        /// </summary>
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var previous = await _contractService.GetContractByIdAsync(id);
            if (previous == null) return NotFound();
            var previousJson = System.Text.Json.JsonSerializer.Serialize(previous);
            var deleted = await _contractService.DeleteContractAsync(id);
            if (!deleted) return NotFound();
            await _historyService.AddHistoryAsync(new ContractHistoryDto {
                ContractId = id,
                Action = "Deleted",
                PreviousVersionJson = previousJson,
                Timestamp = System.DateTime.UtcNow
            });
            return NoContent();
        }

        /// <summary>
        /// Assign benefits to a contract
        /// </summary>
        [HttpPost("{id}/assign-benefits")]
        public async Task<IActionResult> AssignBenefits(string id, [FromBody] string benefits)
        {
            var previous = await _contractService.GetContractByIdAsync(id);
            if (previous == null) return NotFound();
            var previousJson = System.Text.Json.JsonSerializer.Serialize(previous);
            var result = await _contractService.AssignBenefitsAsync(id, benefits);
            if (!result) return NotFound();
            await _historyService.AddHistoryAsync(new ContractHistoryDto {
                ContractId = id,
                Action = "BenefitsAssigned",
                PreviousVersionJson = previousJson,
                Timestamp = System.DateTime.UtcNow
            });
            return Ok();
        }

        /// <summary>
        /// Calculate total contributions for an event
        /// </summary>
        [HttpGet("event/{eventId}/total-contribution")]
        public async Task<ActionResult<decimal>> CalculateTotalContribution(string eventId)
        {
            var total = await _contractService.CalculateTotalContributionByEventAsync(eventId);
            return Ok(total);
        }
    }
}