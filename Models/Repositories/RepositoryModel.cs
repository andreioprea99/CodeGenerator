using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class RepositoryModel : BaseComponentModel
    {
        [Required]
        public List<string> DTOs { get; set; } = new List<string>();
    }
}
