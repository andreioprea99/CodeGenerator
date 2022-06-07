using CodeGenerator.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeGenerator.Generator
{
    public class MainGenerator
    {
        private readonly ILogger<Object> _logger;

        public MainGenerator(ILogger<object> logger)
        {
            _logger = logger;
        }

        public void Generate(MainRequestDTO specs, string path)
        {
            if (specs.Entities != null)
            {
                GenerateEntities(specs.Entities, Path.Combine(path, "Entities"));
            }
            if (specs.DTOs != null)
            {
                GenerateDTOs(specs.DTOs, Path.Combine(path, "DTOs"));
            }
            if (specs.Controllers != null)
            {
                GenerateControllers(specs.Controllers, Path.Combine(path, "Controllers"));
            }
            if (specs.Repositories != null)
            {
                GenerateRepositories(specs.Repositories, Path.Combine(path, "Repositories"));
            }
            if (specs.Services != null)
            {
                GenerateServices(specs.Services, Path.Combine(path, "Services"));
            }
            if (specs.Microservices != null)
            {
                GenerateMicroservices(specs.Microservices, Path.Combine(path, "Microservices"));
            }
        }

        private void GenerateEntities(List<EntityModel> entities, string path)
        {
            _logger.LogInformation($"Generating entities at {path}...");
            Directory.CreateDirectory(path);
        }

        private void GenerateDTOs(List<DTOModel> DTOs, string path)
        {
            _logger.LogInformation($"Generating DTOs at {path}...");
            Directory.CreateDirectory(path);
        }

        private void GenerateControllers(List<ControllerModel> controllers, string path)
        {
            _logger.LogInformation($"Generating controllers at {path}...");
            Directory.CreateDirectory(path);
        }

        private void GenerateRepositories(List<RepositoryModel> repositories, string path)
        {
            _logger.LogInformation($"Generating repositories at {path}...");
            Directory.CreateDirectory(path);
        }

        private void GenerateServices(List<ServiceModel> services, string path)
        {
            _logger.LogInformation($"Generating services at {path}...");
            Directory.CreateDirectory(path);
        }

        private void GenerateMicroservices(List<MicroserviceModel> microservices, string path)
        {
            _logger.LogInformation($"Generating microservices at {path}...");
            Directory.CreateDirectory(path);
        }
    }
}
