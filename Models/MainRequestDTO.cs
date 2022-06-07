using System.Collections.Generic;

namespace CodeGenerator.Models
{
    public class MainRequestDTO
    {
        public List<EntityModel> Entities { get; set; }
        public List<DTOModel> DTOs { get; set; }
        public List<ControllerModel> Controllers { get; set; }
        public List<RepositoryModel> Repositories { get; set; }
        public List<ServiceModel> Services { get; set; }
        public List<MicroserviceModel> Microservices { get; set; }
    }
}
