using CodeGenerator.Generator;
using CodeGenerator.Models;
using CodeGenerator.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenerateCodeController : ControllerBase
    {
        private readonly ILogger<MainRequest> _logger;
        private readonly MongoDBService _mongoDBService;
        private readonly IWebHostEnvironment _environment;
        private readonly Regex validId = new Regex("^[0-9A-Fa -f]{24}$");
        private readonly MainGenerator _generator;

        public GenerateCodeController(ILogger<MainRequest> logger, MongoDBService mongoDBService, IWebHostEnvironment environment, MainGenerator generator)
        {
            _logger = logger;
            _mongoDBService = mongoDBService;
            _environment = environment;
            _generator = generator;
        }

        [HttpPost]
        [Consumes("application/json")]
        public async Task<IActionResult> PostRequest([FromBody] MainRequestDTO request)
        {
            var id = await _mongoDBService.InsertRequestAsync(request);
            return CreatedAtAction(nameof(GetRequestByID), new { id }, request);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MainRequest>> GetRequestByID([FromRoute] String id)
        {
            if (!validId.Match(id).Success)
            {
                return BadRequest(new { error = "The id should be a hex string of 24 characters." });
            }

            var specs = await _mongoDBService.GetRequestByID(id);
            if (specs == null)
                return NotFound();
            string path = Path.Combine(_environment.ContentRootPath, $"../GeneratedCode/{id}/");
            _logger.LogInformation($"File created at {path}");
            // Create directory for the generated project
            Directory.CreateDirectory(path);
            _generator.Generate(specs.Request, path);
            return specs;
        }
    }
}
