using CodeGenerator.Constants;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodeGenerator.Models
{
    public class EntityFieldReferenceModel
    {
        public string ReferencedEntity { get; set; } = null!;
        public string ReferencedField { get; set; } = null!;
        public string Type { get; set; } = EntityFieldReferenceType.OneToOne;
    }
}
