using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Challenge_1_Build_APIS.Controllers.Resources
{
    public class SaveVehicleResource
    {
        public int Id { get; set; }

        [Required]
        public int ModelId { get; set; }

        public bool IsRegistered { get; set; }

        public string? Code { get; set; } 

        public string MakeName { get; set; } = null!;

        public string ModelName { get; set; } = null!;

        [Required]
        public ContactResource Contact { get; set; } = new ContactResource();

        public ICollection<int> Features { get; set; } = new Collection<int>();
    }
}
