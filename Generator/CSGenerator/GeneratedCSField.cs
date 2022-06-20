namespace CodeGenerator.Generator
{
    public class GeneratedCSField
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string AccessModifier { get; set; }

        public GeneratedCSField(string type, string name, string accessModifier = "public")
        {
            Type = type;
            Name = name;
            AccessModifier = accessModifier;
        }

        public override string ToString()
        {
            return $"{AccessModifier} {Type} {Name} " + "{ get; set; }";
        }
    }
}
