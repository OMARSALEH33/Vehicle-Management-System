
using System.ComponentModel.DataAnnotations;

namespace Challenge_1_Build_APIS.Core.Models
{
    public class Feature
{
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;
}
}