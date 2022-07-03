using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class ControllerRouteModel
    {
        public enum RouteAction { get, post, put, delete };
        [Required]
        public string Path { get; set; }
        public RouteAction Type { get; set; } = RouteAction.get;
        [Required]
        public string ResponseType { get; set; }

        public List<RouteParameterModel> Parameters { get; set; } = new List<RouteParameterModel>();
    }
}