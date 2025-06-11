using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.API.Controllers
{
    [ApiController]
    [Route("api/contract-history")]
    public class ContractHistoryController : ControllerBase
    {
        private readonly IContractHistoryService _historyService;

        public ContractHistoryController(IContractHistoryService historyService)
        {
            _historyService = historyService;
        }

        /// <summary>
        /// Get contract history by contract id
        /// </summary>
        [HttpGet("{contractId}")]
        public async Task<ActionResult<IEnumerable<ContractHistoryDto>>> GetHistory(string contractId)
        {
            var history = await _historyService.GetHistoryByContractIdAsync(contractId);
            return Ok(history);
        }
    }
}