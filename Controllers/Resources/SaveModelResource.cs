using System.ComponentModel.DataAnnotations;

namespace Challenge_1_Build_APIS.Controllers.Resources
{
    public class SaveModelResource
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
        public int MakeId { get; set; }

    }
}
