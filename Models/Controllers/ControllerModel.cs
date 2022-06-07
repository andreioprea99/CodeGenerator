using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class ControllerModel
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
