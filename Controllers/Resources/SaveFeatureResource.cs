using System.ComponentModel.DataAnnotations;

namespace Challenge_1_Build_APIS.Controllers.Resources
{
    public class SaveFeatureResource 
    {
    
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;
    }
}
