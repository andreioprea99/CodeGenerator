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

        public GeneratedCSMethod(string returnType, string name, Dictionary<string, string> parameters = null, string accessModifier = "public")
        {
            ReturnType = returnType;
            AccessModifier = accessModifier;
            Name = name;
            Parameters = parameters;
        }

        public override string ToString()
        {
            return $"{AccessModifier} {ReturnType} {Name} ({string.Join(", ", Parameters.Select(pair => pair.Key + ' ' + pair.Value))}) " + "\n" +
                "{\n" +
                    "throw new NotImplementedException();\n" +
                "}\n";
        }
    }
}
