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
        public string Name { get; set; } = null!;
        public List<EntityFieldModel> Fields { get; set; } = null!;
    }
}
