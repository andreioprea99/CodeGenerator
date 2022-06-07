using CodeGenerator.Constants;

namespace CodeGenerator.Models
{
    public class EntityFieldReferenceModel
    {
        public string ReferencedEntity { get; set; } = null!;
        public string ReferencedField { get; set; } = null!;
        public string Type { get; set; } = EntityFieldReferenceType.OneToOne;
    }
}
