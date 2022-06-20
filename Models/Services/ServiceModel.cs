using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class ServiceModel
    {
        [Required]
        public string Name { get; set; } = null!;
        public List<string> Repositories { get; set; }
    }
}
