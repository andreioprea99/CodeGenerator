using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.Models
{
    public class EntityModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("Name")]
        public string Name { get; set; } = null!;
    }
}
