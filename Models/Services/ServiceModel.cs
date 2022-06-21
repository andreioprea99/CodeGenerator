using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class ServiceModel : BaseComponentModel
    {
        public List<string> Repositories { get; set; } = new List<string>();
    }
}
