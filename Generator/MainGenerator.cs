using CodeGenerator.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Generator
{
    public class MainGenerator
    {
        private readonly ILogger<Object> _logger;
        Dictionary<Language, IGenerator> _generators;

        public MainGenerator(ILogger<MainGenerator> logger, IEnumerable<IGenerator> generators)
        {
            _logger = logger;
            _generators = generators.ToDictionary(generator => generator.Type, generator => generator);
        }

        public async Task Generate(MainRequestDTO request, string path)
        {
            await _generators[request.Type].Generate(request, path);
        }
    }
}
