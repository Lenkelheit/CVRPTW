using System;
using System.ComponentModel;

namespace Domains.Models.Output
{
    public class Summary
    {
        [DisplayName("Vehicle name")]
        public string VehicleName { get; set; }
        [DisplayName("Total distance")]
        public double TotalDistance { get; set; }

        [DisplayName("Total duration")]
        public DateTime TotalDuration => TravellingTime + WaitingTime.TimeOfDay + ServiceTime.TimeOfDay;
        [DisplayName("Travelling time")]
        public DateTime TravellingTime { get; set; }
        [DisplayName("Waiting time")]
        public DateTime WaitingTime { get; set; }
        [DisplayName("Service time")]
        public DateTime ServiceTime { get; set; }

        [DisplayName("Max volume")]
        public double MaxVolume { get; set; }
        [DisplayName("Max weight")]
        public double MaxWeight { get; set; }

        [DisplayName("Number of visits")]
        public int NumberOfVisits { get; set; }
        [DisplayName("Number of unassigned orders")]
        public int NumberOfUnassignedOrders { get; set; }
    }
}
