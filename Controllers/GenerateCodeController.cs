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
        public async Task<IActionResult> PostRequest([FromBody] MainRequest request)
        {
            await _mongoDBService.InsertRequestAsync(request);
            return CreatedAtAction(nameof(GetRequestByID), new { id = request.Id }, request);
        }

        [HttpGet("{id}")]
        public async Task<MainRequest> GetRequestByID(String id)
        {
            var result = await _mongoDBService.GetRequestByID(id);
            if (result.Count == 0)
                return null;
            return result[0];
        }
    }
}
