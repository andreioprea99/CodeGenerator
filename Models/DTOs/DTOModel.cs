using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeGenerator.Models
{
    public enum DTOType
    {
        Read,
        Insert,
        Update
    }
    public class DTOModel : BaseComponentModel
    {
        public DTOType Type { get; set; } = DTOType.Read;
        [Required]
        public List<DTOFieldModel> Fields { get; set; } = null!;
    }
}
