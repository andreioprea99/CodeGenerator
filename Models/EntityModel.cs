using System.Collections.Generic;

namespace CodeGenerator.Models
{
    public class EntityModel
    {
        public string Name { get; set; } = null!;
        public List<EntityFieldModel> Fields { get; set; } = null!;
    }
}
