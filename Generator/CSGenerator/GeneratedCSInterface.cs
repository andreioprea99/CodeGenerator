using System.Collections.Generic;
using System.Linq;


namespace CodeGenerator.Generator
{
    public class GeneratedCSInterface
    {
        public string Name { get; set; }
        public string Namespace { get; set; } = "External";
        public HashSet<string> ImportDependencies { get; } = new HashSet<string> { "System" };
        public List<string> MethodsSignatures { get; }

        public GeneratedCSInterface(GeneratedCSClass generatedClass)
        {
            Name = $"I{generatedClass.Name}";
            Namespace = generatedClass.Namespace;
            MethodsSignatures = generatedClass.Methods.Select(method => method.ExtractMethodSignature()).ToList();
            generatedClass.ImplementsInterface = true;
        }

        public override string ToString()
        {
            var methods = string.Join(";\n", MethodsSignatures) + "\n";
            return "namespace " + Namespace + "\n"
                + "{\n"
                + "public interface " + Name + "\n"
                + "{\n"
                + methods
                + "}\n"
                + "}\n"
                ;
        }
    }
}
