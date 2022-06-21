using CodeGenerator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeGenerator.Generator
{
    public interface IGenerator
    {
        Language Type { get; }
        public Task Generate(MainRequestDTO specs, string path);
        public Task<List<BaseGeneratedClass>> GenerateEntities(MainRequestDTO specs);
        public Task<List<BaseGeneratedClass>> GenerateDTOs(MainRequestDTO specs);
        public Task<List<BaseGeneratedClass>> GenerateControllers(MainRequestDTO specs);
        public Task<List<BaseGeneratedClass>> GenerateRepositories(MainRequestDTO specs);
        public Task<List<BaseGeneratedClass>> GenerateServices(MainRequestDTO specs);
        public Task<List<BaseGeneratedClass>> WriteMicroservices(MainRequestDTO specs, Dictionary<string, List<BaseGeneratedClass>> generatedClasses, string path);
    }
}
