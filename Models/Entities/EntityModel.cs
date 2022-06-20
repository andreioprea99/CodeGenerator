using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class EntityModel
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public List<EntityFieldModel> Fields { get; set; } = null!;

        public string ToStringWithDependencies(string dependencies)
        {
            string fields = string.Join("\n", Fields);
            return $"class {Name} \n" +
                "{\n" +
                    fields + "\n\n" +
                    dependencies + "\n" +
                "}";
        }
    }

    
}
