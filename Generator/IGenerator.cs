using CodeGenerator.Models;
using System.Threading.Tasks;

namespace CodeGenerator.Generator
{
    public interface IGenerator
    {
        Language Type { get; }
        public Task Generate(MainRequestDTO specs, string path);
    }
}
