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
                generatedClasses[entitiesDictKey] = await GenerateEntities(specs);
            }
            if (specs.DTOs != null)
            {
                generatedClasses[dtosDictKey] = await GenerateDTOs(specs, generatedClasses);
            }

            if (specs.Repositories != null)
            {
                generatedClasses[repositoriesDictKey] = await GenerateRepositories(specs, generatedClasses);
            }
            if (specs.Services != null)
            {
                generatedClasses[servicesDictKey] = await GenerateServices(specs, generatedClasses);
            }
            if (specs.Controllers != null)
            {
                await GenerateControllers(specs, generatedClasses);
            }
            if (specs.Microservices != null)
            {
                await GenerateMicroservices(specs, generatedClasses, path);
            }
            await WriteClasses(generatedClasses, path);
        }

        public async Task<List<GeneratedCSClass>> GenerateEntities(MainRequestDTO specs)
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
        public async Task<List<GeneratedCSClass>> GenerateDTOs(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses)
        {
            _logger.LogInformation($"Generating DTOs...");
            List<GeneratedCSClass> generatedDTOs = new List<GeneratedCSClass>();
            Dictionary<string, EntityModel> nameEntityMapping = specs.Entities.ToDictionary(entity => entity.Name, entity => entity);
            foreach (var dto in specs.DTOs)
            {
                var generatedDTO = new GeneratedCSClass { Name = dto.Name };
                foreach (var dtoField in dto.Fields)
                {
                    var field = nameEntityMapping[dtoField.Projecting.EntityName].Fields.Where(field => field.Name == dtoField.Projecting.FieldName).First();
                    generatedDTO.AddField(field.Type, dtoField.Name);
                }
                generatedDTOs.Add(generatedDTO);
            }
            return generatedDTOs;
        }

        public async Task<List<GeneratedCSClass>> GenerateControllers(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses)
        {
            _logger.LogInformation($"Generating controllers...");

            return new List<GeneratedCSClass>();
        }

        public async Task<List<GeneratedCSClass>> GenerateRepositories(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses)
        {
            _logger.LogInformation($"Generating repositories...");
            List<GeneratedCSClass> generatedRepositories = new List<GeneratedCSClass>();
            Dictionary<string, DTOModel> nameDTOMapping = specs.DTOs.ToDictionary(dto => dto.Name, dto => dto);
            foreach (var repository in specs.Repositories)
            {
                var generatedRepository = new GeneratedCSClass { Name = repository.Name };
                foreach (var dtoDependencyName in repository.DTOs)
                {
                    var dto = nameDTOMapping[dtoDependencyName];
                    switch (dto.Type)
                    {
                        case DTOType.Read:
                            generatedRepository.AddMethod(dto.Name, $"Get{dto.Name}ById", new Dictionary<string, string> { { "long", "id" } });
                            break;
                        case DTOType.Insert:
                            generatedRepository.AddMethod("void", $"Insert{dto.Name}", new Dictionary<string, string> { { dto.Name, $"{dto.Name}Object" } });
                            break;
                        case DTOType.Update:
                            generatedRepository.AddMethod("void", $"Update{dto.Name}", new Dictionary<string, string> { { dto.Name, $"{dto.Name}Object" } });
                            break;
                    }
                }
                generatedRepositories.Add(generatedRepository);
            }
            return generatedRepositories;
        }

        public async Task<List<GeneratedCSClass>> GenerateServices(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses)
        {
            _logger.LogInformation($"Generating services at...");
            List<GeneratedCSClass> generatedServices = new List<GeneratedCSClass>();
            Dictionary<string, RepositoryModel> nameRepositoryMapping = specs.Repositories.ToDictionary(repository => repository.Name, repository => repository);
            foreach (var service in specs.Services)
            {
                var generatedService = new GeneratedCSClass { Name = service.Name };
                foreach (var repositoryName in service.Repositories)
                {
                    generatedService.AddConstructorField(repositoryName, $"{repositoryName}Object");
                }
                generatedServices.Add(generatedService);
            }
            return generatedServices;
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
                    var directoryFilePath = Path.Combine(rootPath, generatedClass.Namespace, entry.Key);
                    Directory.CreateDirectory(directoryFilePath);
                    await File.WriteAllTextAsync(Path.Combine(directoryFilePath, $"{generatedClass.Name}.cs"), generatedClass.ToString());
                }
            }
            //Format generated code files
            await MainGenerator.InvokePSCommand("dotnet", $"format -w {rootPath} --folder");
        }


    }
}
