using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class ServiceModel
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
