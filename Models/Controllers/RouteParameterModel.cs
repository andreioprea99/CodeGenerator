using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class RouteParameterModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
    }
}