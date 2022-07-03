using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class RestClientModel : BaseComponentModel
    {
        [Required]
        public string For { get; set; }
    }
}
