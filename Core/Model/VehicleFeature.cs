using Challenge_1_Build_APIS.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Challenge_1_Build_APIS.Core.Models;

[Table("VehicleFeatures")]
public class VehicleFeature
{
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    public int FeatureId { get; set; }
    public Feature Feature { get; set; } = null!;
}
