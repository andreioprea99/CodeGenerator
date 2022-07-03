using CodeGenerator.Generator;
using CodeGenerator.Models;
using CodeGenerator.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.IO.Compression;
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
            var id = "";
            try
            {
                id = await _mongoDBService.InsertRequestAsync(request);
            }
            catch
            {
                return StatusCode(429);
            }
            
            string path = Path.Combine(_environment.ContentRootPath, $"../GeneratedCode/{id}/");
            _logger.LogInformation($"Source files for {id} created at {path}");
            // Create directory for the generated project
            Directory.CreateDirectory(path);
            await _generator.Generate(request, path);
            return CreatedAtAction(nameof(GetByID), new { id }, request);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> PostIdRequest([FromRoute] String id)
        {
            MainRequest oldRequest;
            if (!validId.Match(id).Success)
            {
                return BadRequest(new { error = "The id should be a hex string of 24 characters." });
            }
            try
            {
                oldRequest = await _mongoDBService.GetRequestByID(id);
            }
            catch
            {
                return NotFound(new { error = $"The request with the id {id} doesn't exist" });
            }

            string path = Path.Combine(_environment.ContentRootPath, $"../GeneratedCode/{id}/");
            _logger.LogInformation($"Source files for {id} created at {path}");
            // Create directory for the generated project
            Directory.CreateDirectory(path);
            await _generator.Generate(oldRequest.Request, path);
            return CreatedAtAction(nameof(GetByID), new { id }, oldRequest.Request);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByID([FromRoute] String id)
        {
            if (!validId.Match(id).Success)
            {
                return BadRequest(new { error = "The id should be a hex string of 24 characters." });
            }
            string fileName = $"{id}.zip";
            string sourcePath = Path.Combine(_environment.ContentRootPath, $"../GeneratedCode/{id}/");
            string destinationPath = Path.Combine(_environment.ContentRootPath, $"../GeneratedCode/{fileName}");
            if (!Directory.Exists(sourcePath))
            {
                return NotFound(new { error = "The id is not registered" });
            }
            // Delete old archive if exists
            System.IO.File.Delete(destinationPath);

            ZipFile.CreateFromDirectory(sourcePath, destinationPath);
            return File(System.IO.File.ReadAllBytes(destinationPath), "application/zip", fileName);
        }
        [HttpPost("{id}/git")]
        public async Task<IActionResult> PostGithub([FromRoute] String id, [FromBody] GitRequestModel gitDetails)
        {
            if (!validId.Match(id).Success)
            {
                return BadRequest(new { error = "The id should be a hex string of 24 characters." });
            }
            string path = Path.Combine(_environment.ContentRootPath, $"../GeneratedCode/{id}/");
            if (!Directory.Exists(path))
            {
                return NotFound(new { error = "The id is not registered" });
            }
            try
            {
                await MainGenerator.PushInGit(gitDetails, path);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }
    }
}
