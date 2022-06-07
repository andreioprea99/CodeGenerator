using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class MicroserviceModel
    {
        [Required]
        public string Name { get; set; } = "dto_name";
        public List<string> Repositories { get; set; }
        public List<string> Services { get; set; }
        public List<string> Contollers { get; set; }
    }
}
