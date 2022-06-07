using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public enum EntityFieldReferenceType
    {
        OneToOne,
        ManyToOne,
        ManyToMany
    }
    public class EntityFieldReferenceModel
    {
        [Required]
        public string ReferencedEntity { get; set; } = null!;
        [Required]
        public string ReferencedField { get; set; } = null!;
        [Required]
        public EntityFieldReferenceType Type { get; set; } = EntityFieldReferenceType.OneToOne;
    }
}
