using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class MicroserviceModel : BaseComponentModel
    {
        public List<string> Entities { get; set; } = new List<string>();
        public List<string> DTOs { get; set; } = new List<string>();
        public List<string> Repositories { get; set; } = new List<string>();
        public List<string> Services { get; set; } = new List<string>();
        public List<string> Controllers { get; set; } = new List<string>();
        public List<string> RestClients { get; set; } = new List<string>();
    }
}
