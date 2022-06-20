using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.Generator
{
    public class GeneratedCSMethod
    {
        public string ReturnType { get; set; }
        public string AccessModifier { get; set; }
        public string Name { get; set; }
        public Dictionary<string,string> Parameters { get; set; }
        public List<string> Annotations { get; set; } = new List<string>();

        public GeneratedCSMethod(string returnType, string name, Dictionary<string, string> parameters = null, string accessModifier = "public")
        {
            ReturnType = returnType;
            AccessModifier = accessModifier;
            Name = name;
            Parameters = parameters;
        }

        public string ExtractMethodSignature()
        {
            return $"{AccessModifier} {ReturnType} {Name} ({string.Join(", ", Parameters.Select(pair => pair.Key + ' ' + pair.Value))})";
        }

        public override string ToString()
        {
            return  string.Join("\n", Annotations) + "\n" + ExtractMethodSignature() + "\n" +
                "{\n" +
                    "throw new NotImplementedException();\n" +
                "}\n";
        }
    }
}
