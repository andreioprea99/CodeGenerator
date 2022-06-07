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
    }
}
