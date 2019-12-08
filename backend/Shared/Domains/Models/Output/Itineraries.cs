using System;
using System.ComponentModel;

namespace Domains.Models.Output
{
    public class Itineraries
    {
        [DisplayName("Vehicle name")]
        public string VehicleName { get; set; }
        [DisplayName("Load")]
        public int Load { get; set; }
        [DisplayName("Location min time")]
        public DateTime From  { get; set; }
        [DisplayName("Location max time")]
        public DateTime To { get; set; }
        [DisplayName("Distance")]
        public int Distance { get; set; }
    }
}
