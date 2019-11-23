using System.Collections.Generic;

namespace Domains.Models.Input
{
    public class FileInput
    {
        public IEnumerable<Distances> Distances { get; set; }
        public IEnumerable<Locations> Locations { get; set; }
        public IEnumerable<Orders> Orders { get; set; }
        public IEnumerable<Vehicles> Vehicles { get; set; }
    }
}
