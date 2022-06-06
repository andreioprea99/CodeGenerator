using CodeGenerator.Models;
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

        public GenerateCodeController(ILogger<MainRequest> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<MainRequest>> Post(MainRequest request)
        {
            return request;
        }
    }
}
