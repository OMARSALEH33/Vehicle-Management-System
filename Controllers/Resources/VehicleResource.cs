using System.Collections.ObjectModel;

namespace Challenge_1_Build_APIS.Controllers.Resources
{
    public class VehicleResource : KeyValuePairResource
    {
        

        public KeyValuePairResource Model { get; set; } = null!;

        public KeyValuePairResource Make { get; set; } = null!;

        public bool IsRegistered { get; set; }

        public ContactResource Contact { get; set; } = null!;

        public DateTime LastUpdate { get; set; }

        public ICollection<KeyValuePairResource> Features { get; set; } = new Collection<KeyValuePairResource>();

    }
}
