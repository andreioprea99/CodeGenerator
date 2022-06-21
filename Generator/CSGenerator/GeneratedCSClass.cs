using CodeGenerator.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeGenerator.Generator
{
    public class GeneratedCSClass : BaseGeneratedClass
    { 
        public bool ImplementsInterface { get; set; } = false;
        private Dictionary<string, string> constructorProperties;
        public HashSet<string> ImportDependencies { get; } = new HashSet<string> { "System" };
        public List<GeneratedCSField> Fields { get; } = new List<GeneratedCSField>();
        public List<GeneratedCSMethod> Methods { get; } = new List<GeneratedCSMethod>();
        public GeneratedCSClass()
        {
        }

        public GeneratedCSClass(EntityModel entity)
        {
            Name = entity.Name;
            foreach (var entityField in entity.Fields)
            {
                AddField(entityField.Type, entityField.Name);
            }
        }
        public void AddField (EntityFieldType type, string name, string accessModifier = "public")
        {
            Fields.Add(new GeneratedCSField(ReturnCSFieldType(type), name, accessModifier));
        }
        // Allow object fields
        public void AddField (string type, string name, string accessModifier = "public")
        {
            Fields.Add(new GeneratedCSField(ReturnCSFieldType(type), name, accessModifier));
        }

        // Convert enum to string
        private  string ReturnCSFieldType(EntityFieldType type)
        {
            switch (type)
            {
                case EntityFieldType.String:
                case EntityFieldType.Int:
                case EntityFieldType.Long:
                    return type.ToString().ToLower();
                case EntityFieldType.DateTime:
                    return type.ToString();
                case EntityFieldType.Boolean:
                    return "bool";
                default:
                    return type.ToString();
            }
        }
        // If it's a custom tyype return the type
        private static string ReturnCSFieldType(string type)
        {
            return type;
        }

        public void AddMethod (string returnType, string name, Dictionary<string, string> parameters = null, string accessModifier = "public")
        {
            Methods.Add(new GeneratedCSMethod(returnType, name, parameters, accessModifier));
        }

        public void AddImportDependency (string dependency)
        {
            ImportDependencies.Add(dependency);
        }

        public void AddConstructorField(string type, string name)
        {
            if (constructorProperties == null)
            {
                constructorProperties = new Dictionary<string, string>();
            }
            constructorProperties.Add($"I{type}", name);
            Fields.Add(new GeneratedCSField($"I{type}", name, "private"));
        }

        private string GenerateConstructor()
        {
            if (constructorProperties == null)
                return "";
            return $"public {Name} ({string.Join(", ", constructorProperties.Select(pair => pair.Key + ' ' + pair.Value))})"
                + "{\n"
                + $"{ string.Join("\n", constructorProperties.Values.Select(name => $"this.{name} = {name};" )) }"
                + "}\n";
        }

        public override string ToString()
        {
            var dependencies = string.Join("\n", ImportDependencies.Select(dependency => $"using {dependency};"));
            var fields = string.Join("\n", Fields) + "\n"; 
            var methods = string.Join("\n", Methods) + "\n";
            return dependencies + "\n"
                + "namespace " + Namespace + "\n"
                + "{\n"
                + "public class " + Name + $"{(ImplementsInterface ? $" : I{Name}" : "")}" + "\n"
                + "{\n"
                + fields
                + GenerateConstructor()
                + methods
                + "}\n"
                + "}\n"
                ;
        }
    }
}
