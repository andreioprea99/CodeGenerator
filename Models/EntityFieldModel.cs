using CodeGenerator.Constants;
using System.Collections.Generic;

namespace CodeGenerator.Models
{
    public class EntityFieldModel
    {
        public string Type { get; set; } = EntityFieldType.Int;
        public string Name { get; set; } = "fieldName";
        public bool PrimaryKey { get; set; } = false;
        public bool UniqueKey { get; set; } = false;
        public uint? MaxSize { get; set; } = 255;
        public bool NotNull { get; set; } = false;
        public List<EntityFieldReferenceModel> References { get; set; } = null!;
    }
}