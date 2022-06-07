using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class RepositoryModel
    {
        [Required]
        public List<string> DTOs { get; set; } = null!;
    }
}
