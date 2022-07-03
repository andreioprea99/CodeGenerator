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
        public List<string> MethodAnnotations { get; set; } = new List<string>();

        public GeneratedCSMethod(string returnType, string name, Dictionary<string, string> parameters = null, string accessModifier = "public")
        {
            ReturnType = returnType;
            AccessModifier = accessModifier;
            Name = name;
            Parameters = parameters;
        }

        public static GeneratedCSMethod CopyMethod(GeneratedCSMethod orignialMethod)
        {
            return new GeneratedCSMethod(orignialMethod.ReturnType, orignialMethod.Name, orignialMethod.Parameters, orignialMethod.AccessModifier);
        }
        public string ExtractMethodSignature()
        {
            return $"{AccessModifier} {ReturnType} {Name} ({string.Join(", ", Parameters.Select(pair => pair.Value + ' ' + pair.Key))})";
        }

        public override string ToString()
        {
            return  string.Join("\n", MethodAnnotations) + "\n" 
                + ExtractMethodSignature() + "\n" +
                "{\n" +
                    "throw new NotImplementedException();\n" +
                "}\n";
        }
    }
}
