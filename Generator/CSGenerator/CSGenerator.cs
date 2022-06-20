using CodeGenerator.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Generator
{
    public class CSGenerator : IGenerator
    {
        private readonly ILogger _logger;
        private const string entitiesDictKey = "Entities";
        private const string dtosDictKey = "DTOs";
        private const string repositoriesDictKey = "Repositories";
        private const string servicesDictKey = "Services";
        private const string controllersDictKey = "Controllers";
        private const string microServicesDictKey = "Microservices";
        public Language Type { get; } = Language.CS;

        public CSGenerator(ILogger<CSGenerator> logger)
        {
            _logger = logger;
        }

        public async Task Generate(MainRequestDTO specs, string path)
        {
            Dictionary<string, List<GeneratedCSClass>> generatedClasses = new Dictionary<string, List<GeneratedCSClass>>();
            if (specs.Entities != null)
            {
                generatedClasses[entitiesDictKey] = await GenerateEntities(specs, Path.Combine(path, entitiesDictKey));
            }
            if (specs.DTOs != null)
            {
                generatedClasses[dtosDictKey] = await GenerateDTOs(specs, generatedClasses, Path.Combine(path, dtosDictKey));
            }
            if (specs.Controllers != null)
            {
                await GenerateControllers(specs, generatedClasses, Path.Combine(path, "Controllers"));
            }
            if (specs.Repositories != null)
            {
                await GenerateRepositories(specs, generatedClasses, Path.Combine(path, "Repositories"));
            }
            if (specs.Services != null)
            {
                await GenerateServices(specs, generatedClasses, Path.Combine(path, "Services"));
            }
            if (specs.Microservices != null)
            {
                await GenerateMicroservices(specs, generatedClasses, Path.Combine(path, "Microservices"));
            }
            await WriteClasses(generatedClasses, path);
        }

        public async Task<List<GeneratedCSClass>> GenerateEntities(MainRequestDTO specs, string path)
        {
            _logger.LogInformation($"Generating entities...");
            // Transform list in dictionary to ease dependency computing
            Dictionary<string, GeneratedCSClass> generatedEntities = specs.Entities.ToDictionary(entity => entity.Name, entity => new GeneratedCSClass(entity));
            ComputeDependencies(specs.Entities, generatedEntities);

            return generatedEntities.Values.ToList();
        }
        private void ComputeDependencies(List<EntityModel> entities, Dictionary<string, GeneratedCSClass> generatedEntities)
        {
            foreach (var entity in entities)
            {
                foreach (var field in entity.Fields)
                {
                    foreach (var reference in field.References)
                    {
                        switch (reference.Type)
                        {
                            case EntityFieldReferenceType.OneToOne:
                                generatedEntities[entity.Name].AddField(reference.ReferencedEntity, $"{reference.ReferencedEntity}Navigation");
                                try
                                {
                                    generatedEntities[reference.ReferencedEntity].AddField(entity.Name, $"{entity.Name}Navigation");
                                }
                                catch (Exception e)
                                {
                                    _logger.LogDebug(e.Message);
                                }
                                break;
                            case EntityFieldReferenceType.ManyToOne:
                                generatedEntities[entity.Name].AddField(reference.ReferencedEntity, $"{reference.ReferencedEntity}Navigation");
                                try
                                {
                                    generatedEntities[reference.ReferencedEntity].AddImportDependency("System.Collections.Generic");
                                    generatedEntities[reference.ReferencedEntity].AddField($"List<{entity.Name}>", $"{entity.Name}Navigation");
                                }
                                catch (Exception e)
                                {
                                    _logger.LogDebug(e.Message);
                                }
                                break;
                            case EntityFieldReferenceType.ManyToMany:
                                generatedEntities[entity.Name].AddImportDependency("System.Collections.Generic");
                                generatedEntities[entity.Name].AddField($"List<{reference.ReferencedEntity}>", $"{reference.ReferencedEntity}Navigation");
                                try
                                {
                                    generatedEntities[reference.ReferencedEntity].AddImportDependency("System.Collections.Generic");
                                    generatedEntities[reference.ReferencedEntity].AddField($"List<{entity.Name}>", $"{entity.Name}Navigation");
                                }
                                catch (Exception e)
                                {
                                    _logger.LogDebug(e.Message);
                                }
                                break;
                        }
                    }
                }
            }
        }
        public async Task<List<GeneratedCSClass>> GenerateDTOs(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses, string path)
        {
            _logger.LogInformation($"Generating DTOs...");
            List<GeneratedCSClass> generatedDTOs = new List<GeneratedCSClass>();
            List<GeneratedCSClass> generatedEntities = generatedClasses[entitiesDictKey];
            foreach (var dto in specs.DTOs)
            {
                var generatedDTO = new GeneratedCSClass { Namespace = "DTOs" , Name = dto.Name };
                foreach (var dtoField in dto.Fields)
                {
                    var field = generatedEntities.Where(entity => entity.Name == dtoField.Projecting.EntityName).First().Fields.Where(field => field.Name == dtoField.Projecting.FieldName).First();
                    generatedDTO.AddField(field.Type, dtoField.Name, field.AccessModifier);
                }
                generatedDTOs.Add(generatedDTO);
            }
            return generatedDTOs;
        }

        public async Task<List<GeneratedCSClass>> GenerateControllers(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses, string path)
        {
            _logger.LogInformation($"Generating controllers...");
            Directory.CreateDirectory(path);
            return new List<GeneratedCSClass>();
        }

        public async Task<List<GeneratedCSClass>> GenerateRepositories(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses, string path)
        {
            _logger.LogInformation($"Generating repositories...");
            Directory.CreateDirectory(path);
            return new List<GeneratedCSClass>();
        }

        public async Task<List<GeneratedCSClass>> GenerateServices(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses, string path)
        {
            _logger.LogInformation($"Generating services at...");
            return new List<GeneratedCSClass>();
        }

        public async Task<List<GeneratedCSClass>> GenerateMicroservices(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses, string path)
        {
            _logger.LogInformation($"Generating microservices...");
            return new List<GeneratedCSClass>();
        }

        private async Task WriteClasses(Dictionary<string, List<GeneratedCSClass>> generatedClasses, string rootPath)
        {
            Directory.CreateDirectory(rootPath);
            foreach (var entry in generatedClasses)
            {
                foreach (var generatedClass in entry.Value)
                {
                    Directory.CreateDirectory(Path.Combine(rootPath, generatedClass.Namespace));
                    await File.WriteAllTextAsync(Path.Combine(rootPath, generatedClass.Namespace, $"{generatedClass.Name}.cs"), generatedClass.ToString());
                }
            }
            //Format generated code files
            await MainGenerator.InvokePSCommand("dotnet", $"format -w {rootPath} --folder");
        }

        
    }
}
