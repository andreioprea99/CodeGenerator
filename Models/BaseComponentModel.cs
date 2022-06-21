using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public abstract class BaseComponentModel
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
