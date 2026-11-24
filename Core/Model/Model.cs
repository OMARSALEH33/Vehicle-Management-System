
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challenge_1_Build_APIS.Core.Models{

    [Table("Models")]

    public class Model
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        public int MakeId { get; set; }
        public Make Make { get; set; } = null!;
    }
}
