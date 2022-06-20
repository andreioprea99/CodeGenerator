using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class DTOFieldModel
    {
        [Required]
        public string Name { get; set; } = "dto_name";
        [Required]
        public DTOFieldModelProjection Projecting { get; set; } = null!;
    }
}