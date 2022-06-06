using CodeGenerator.DTO;
using CodeGenerator.Models;
using CodeGenerator.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenerateCodeController : ControllerBase
    {
        private readonly ILogger<MainRequest> _logger;
        private readonly MongoDBService _mongoDBService;

        public GenerateCodeController(ILogger<MainRequest> logger, MongoDBService mongoDBService)
        {
            _logger = logger;
            _mongoDBService = mongoDBService;
        }

        [HttpPost]
        public async Task<IActionResult> PostRequest([FromBody] MainRequestDTO request)
        {
            var id = await _mongoDBService.InsertRequestAsync(request);
            return CreatedAtAction(nameof(GetRequestByID), new { id }, request);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MainRequest>> GetRequestByID(String id)
        {
            return await _mongoDBService.GetRequestByID(id) ?? new ActionResult<MainRequest>(NotFound());
        }
    }
}
