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
        private const string restClientsDictKey = "RestClients";
        public Language Type { get; } = Language.CS;

        public CSGenerator(ILogger<CSGenerator> logger)
        {
            _logger = logger;
        }

        public async Task Generate(MainRequestDTO specs, string path)
        {
            Dictionary<string, List<BaseGeneratedClass>> generatedFiles = new Dictionary<string, List<BaseGeneratedClass>>();
            if (specs.Entities != null)
            {
                generatedFiles[entitiesDictKey] = await GenerateEntities(specs);
            }
            if (specs.DTOs != null)
            {
                generatedFiles[dtosDictKey] = await GenerateDTOs(specs);
            }

            if (specs.Repositories != null)
            {
                generatedFiles[repositoriesDictKey] = await GenerateRepositories(specs);
            }
            if (specs.Services != null)
            {
                generatedFiles[servicesDictKey] = await GenerateServices(specs);
            }
            if (specs.Controllers != null)
            {
                generatedFiles[controllersDictKey] = await GenerateControllers(specs);
            }
            if (specs.RestClients != null)
            {
                generatedFiles[restClientsDictKey] = await GenerateRestClients(specs, generatedFiles[controllersDictKey]);
            }
            if (specs.Microservices != null)
            {
                await WriteMicroservices(specs, generatedFiles, path);
            }
            await WriteClasses(generatedFiles, path, true);
        }

        public async Task<List<BaseGeneratedClass>> GenerateEntities(MainRequestDTO specs)
        {
            _logger.LogInformation($"Generating entities...");
            // Transform list in dictionary to ease dependency computing
            Dictionary<string, GeneratedCSClass> generatedEntities = specs.Entities.ToDictionary(entity => entity.Name, entity => new GeneratedCSClass(entity));
            ComputeDependencies(specs.Entities, generatedEntities);

            return new List<BaseGeneratedClass>(generatedEntities.Values.ToList());
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
        public async Task<List<BaseGeneratedClass>> GenerateDTOs(MainRequestDTO specs)
        {
            _logger.LogInformation($"Generating DTOs...");
            List<BaseGeneratedClass> generatedDTOs = new List<BaseGeneratedClass>();
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

        public async Task<List<BaseGeneratedClass>> GenerateControllers(MainRequestDTO specs)
        {
            _logger.LogInformation($"Generating controllers...");
            List<BaseGeneratedClass> generatedControllers = new List<BaseGeneratedClass>();

            foreach (var controller in specs.Controllers)
            {
                // Add default controller details
                var generatedController = new GeneratedCSClass { Name = controller.Name };
                generatedController.ClassAnnotations.Add("[ApiController]");
                generatedController.AddImportDependency("Microsoft.AspNetCore.Mvc");
                generatedController.ImplementsInterface = "Controller";
                // Build constructor
                foreach (var serviceName in controller.Services)
                {
                    generatedController.AddConstructorField(serviceName, $"{serviceName}Object");
                }
                // Build methods
                foreach (var route in controller.Routes)
                {
                    string methodName = "", requestTypeAnnotation = "";
                    var parameters = route.Parameters.ToDictionary(parameter => parameter.Name, parameter => parameter.Type);
                    switch (route.Type)
                    {
                        case ControllerRouteModel.RouteAction.get:
                            methodName = "GetBy";
                            requestTypeAnnotation = "[HttpGet]";
                            break;
                        case ControllerRouteModel.RouteAction.post:
                            methodName = "Post";
                            requestTypeAnnotation = "[HttpPost]";
                            break;
                        case ControllerRouteModel.RouteAction.put:
                            methodName = "Put";
                            requestTypeAnnotation = "[HttpPut]";
                            break;
                        case ControllerRouteModel.RouteAction.delete:
                            methodName = "DeleteBy";
                            requestTypeAnnotation = "[HttpDelete]";
                            break;
                    }
                    methodName += string.Join("", parameters.Keys);
                    GeneratedCSMethod generatedMethod = new GeneratedCSMethod(route.ResponseType, methodName, parameters);
                    generatedMethod.MethodAnnotations.Add($"[Route(\"{route.Path}\")]");
                    generatedMethod.MethodAnnotations.Add(requestTypeAnnotation);
                    generatedController.Methods.Add(generatedMethod);
                }
                generatedControllers.Add(generatedController);
            }
            return generatedControllers;
        }

        public async Task<List<BaseGeneratedClass>> GenerateRepositories(MainRequestDTO specs)
        {
            _logger.LogInformation($"Generating repositories...");
            List<BaseGeneratedClass> generatedRepositories = new List<BaseGeneratedClass>();
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
                            generatedRepository.AddMethod(dto.Name, $"Get{dto.Name}ById", new Dictionary<string, string> { { "id", "long" } });
                            break;
                        case DTOType.Insert:
                            generatedRepository.AddMethod("void", $"Insert{dto.Name}", new Dictionary<string, string> { { $"{dto.Name}Object", dto.Name } });
                            break;
                        case DTOType.Update:
                            generatedRepository.AddMethod("void", $"Update{dto.Name}", new Dictionary<string, string> { { $"{dto.Name}Object", dto.Name } });
                            break;
                    }
                }
                generatedRepositories.Add(generatedRepository);
                generatedRepositories.Add(new GeneratedCSInterface(generatedRepository));
            }
            return generatedRepositories;
        }

        public async Task<List<BaseGeneratedClass>> GenerateServices(MainRequestDTO specs)
        {
            _logger.LogInformation($"Generating services at...");
            List<BaseGeneratedClass> generatedServices = new List<BaseGeneratedClass>();
            foreach (var service in specs.Services)
            {
                var generatedService = new GeneratedCSClass { Name = service.Name };
                foreach (var repositoryName in service.Repositories)
                {
                    generatedService.AddConstructorField(repositoryName, $"{repositoryName}Object");
                }
                generatedServices.Add(generatedService);
                generatedServices.Add(new GeneratedCSInterface(generatedService));
            }
            return generatedServices;
        }

        public async Task<List<BaseGeneratedClass>> GenerateRestClients (MainRequestDTO specs, List<BaseGeneratedClass> controllers)
        {
            _logger.LogInformation($"Generating rest clients...");
            List<BaseGeneratedClass> generatedRestClients = new List<BaseGeneratedClass>();
            Dictionary<string, GeneratedCSClass> controllersDictionary = controllers.ToDictionary(controler => controler.Name, controller => (GeneratedCSClass)controller);
            foreach (var restClient in specs.RestClients)
            {
                var generatedRestClient = new GeneratedCSClass { Name = restClient.Name };
                foreach (var method in controllersDictionary[restClient.For].Methods)
                {
                    generatedRestClient.Methods.Add(GeneratedCSMethod.CopyMethod(method));
                }
                generatedRestClients.Add(generatedRestClient);
            }
            return generatedRestClients;
        }
        public async Task<List<BaseGeneratedClass>> WriteMicroservices(MainRequestDTO specs, Dictionary<string, List<BaseGeneratedClass>> generatedClasses, string path)
        {
            _logger.LogInformation($"Writing microservices...");
            foreach (var microservice in specs.Microservices)
            {
                Dictionary<string, List<BaseGeneratedClass>> microserviceGeneratedClasses = new Dictionary<string, List<BaseGeneratedClass>>();

                microserviceGeneratedClasses[entitiesDictKey] = UpdateGeneratedClassesNamespace(microservice.Entities, generatedClasses.GetValueOrDefault(entitiesDictKey), microservice.Name);
                microserviceGeneratedClasses[dtosDictKey] = UpdateGeneratedClassesNamespace(microservice.DTOs, generatedClasses.GetValueOrDefault(dtosDictKey), microservice.Name);
                microserviceGeneratedClasses[repositoriesDictKey] = UpdateGeneratedClassesNamespace(microservice.Repositories, generatedClasses.GetValueOrDefault(repositoriesDictKey), microservice.Name);
                microserviceGeneratedClasses[servicesDictKey] = UpdateGeneratedClassesNamespace(microservice.Services, generatedClasses.GetValueOrDefault(servicesDictKey), microservice.Name);
                microserviceGeneratedClasses[controllersDictKey] = UpdateGeneratedClassesNamespace(microservice.Controllers, generatedClasses.GetValueOrDefault(controllersDictKey), microservice.Name);
                microserviceGeneratedClasses[restClientsDictKey] = UpdateGeneratedClassesNamespace(microservice.RestClients, generatedClasses.GetValueOrDefault(restClientsDictKey), microservice.Name);

                await WriteClasses(microserviceGeneratedClasses, path);
            }
            return new List<BaseGeneratedClass>();
        }

        private static List<BaseGeneratedClass> UpdateGeneratedClassesNamespace(List<string> modelsNames, List<BaseGeneratedClass> generatedClasses, string microserviceName)
        {
            if (generatedClasses == null)
                return new List<BaseGeneratedClass>();
            List<BaseGeneratedClass> result = new List<BaseGeneratedClass>();
            result = generatedClasses.Where(generatedClass => modelsNames.Contains(generatedClass.Name) ||
                                                             (modelsNames.Contains(generatedClass.Name[1..]) && generatedClass is GeneratedCSInterface)).ToList();
            result.AsParallel().ForAll(generatedClass => generatedClass.Namespace = microserviceName);
            return result;
        }

        private static async Task WriteClasses(Dictionary<string, List<BaseGeneratedClass>> generatedClasses, string rootPath, bool usingDefaultNamespace = false)
        {
            if (usingDefaultNamespace)
            {
                generatedClasses.ToDictionary(entry => entry.Key, entry => entry.Value.Where(generatedClass => generatedClass.Namespace == "External"));
            }
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
