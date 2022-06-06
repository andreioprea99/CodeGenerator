using CodeGenerator.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGenerator.DTO
{
    public class MainRequestDTO
    {
        public List<EntityModel> Entities { get; set; }
    }
}
