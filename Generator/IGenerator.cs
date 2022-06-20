using CodeGenerator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeGenerator.Generator
{
    public interface IGenerator
    {
        Language Type { get; }
        public Task Generate(MainRequestDTO specs, string path);
        public Task<List<GeneratedCSClass>> GenerateEntities(MainRequestDTO specs, string path);
        public Task<List<GeneratedCSClass>> GenerateDTOs(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses, string path);
        public Task<List<GeneratedCSClass>> GenerateControllers(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses, string path);
        public Task<List<GeneratedCSClass>> GenerateRepositories(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses, string path);
        public Task<List<GeneratedCSClass>> GenerateServices(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses, string path);
        public Task<List<GeneratedCSClass>> GenerateMicroservices(MainRequestDTO specs, Dictionary<string, List<GeneratedCSClass>> generatedClasses, string path);
    }
}
