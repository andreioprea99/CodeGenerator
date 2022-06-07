using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public class DTOFieldModelProjection
    {
        [Required]
        public string EntityName { get; set; } = "name";
        [Required]
        public string FieldName { get; set; } = "name";
    }
}