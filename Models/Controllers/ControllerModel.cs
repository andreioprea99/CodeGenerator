using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class ControllerModel : BaseComponentModel
    {
        public List<ControllerRouteModel> Routes { get; set; } = new List<ControllerRouteModel>();
        public List<string> Services { get; set; } = new List<string>();
    }
}
