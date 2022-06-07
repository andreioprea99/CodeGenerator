using CodeGenerator.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CodeGenerator.Models
{
    public class MainRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public MainRequestDTO Request { get; set; }
    }
}
