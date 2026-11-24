using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challenge_1_Build_APIS.Core.Models
{
    [Table("Vehicle")]
    public class Vehicle
    {
        internal List<VehicleFeature>? VehicleFeatures;

    
        public int Id { get; set; }
        public int ModelId { get; set; }
        public Model Model { get; set; } = null!;

        public bool IsRegistered { get; set; }

        [Required]
        [StringLength(255)]
        public string ContactName { get; set; } = null!;

        [StringLength(255)]
        public string? ContactEmail { get; set; }

        [Required]
        [StringLength(255)]
        public string ContactPhone { get; set; } = null!;

        public DateTime LastUpdate { get; set; }

        public ICollection<VehicleFeature> Features { get; set; } = new HashSet<VehicleFeature>();
    }
}
