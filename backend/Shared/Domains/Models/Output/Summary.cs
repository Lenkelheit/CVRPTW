using System.ComponentModel;

namespace Domains.Models.Output
{
    public class Summary
    {
        [DisplayName("Vehicle name")]
        public string VehicleName { get; set; }
        [DisplayName("Load")]
        public int Load { get; set; }
        [DisplayName("Distance")]
        public int Distance { get; set; }
        [DisplayName("Total travel minutes")]
        public int Time { get; set; }
        [DisplayName("Number of visits")]
        public int NumberOfVisits { get; set; }
    }
}
