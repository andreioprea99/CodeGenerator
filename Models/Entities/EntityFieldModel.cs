using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public enum EntityFieldType
    {
        Int,
        Long,
        String,
        DateTime,
        Boolean
    }
    public class EntityFieldModel
    {
        public EntityFieldType Type { get; set; } = EntityFieldType.String;
        [Required]
        public string Name { get; set; }
        public bool PrimaryKey { get; set; } = false;
        public bool UniqueKey { get; set; } = false;
        public uint? MaxSize { get; set; } = 255;
        public bool NotNull { get; set; } = false;
        public List<EntityFieldReferenceModel> References { get; set; } = new List<EntityFieldReferenceModel>();

        public override string ToString()
        {
            string fieldType = "";
            switch (Type) {
                case EntityFieldType.Int:
                    fieldType = "int";
                    break;
                case EntityFieldType.String:
                    fieldType = "string";
                    break;
                case EntityFieldType.Long:
                    fieldType = "long";
                    break;
                case EntityFieldType.DateTime:
                    fieldType = "DateTime";
                    break;
                case EntityFieldType.Boolean:
                    fieldType = "bool";
                    break;
            }
            return $"public {fieldType} {Name} " + "{ get; set; }";
        }
    }

}