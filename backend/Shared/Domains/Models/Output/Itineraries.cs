using System.ComponentModel;

namespace Domains.Models.Output
{
    public class Itineraries
    {
        [DisplayName("Vehicle name")]
        public string VehicleName { get; set; }
        [DisplayName("Order name")]
        public string OrderName { get; set; }
        [DisplayName("Location name")]
        public string LocationName { get; set; }
    }
}
