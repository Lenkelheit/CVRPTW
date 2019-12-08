using System;

namespace Domains.Models.Input
{
    public class Locations
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Service { get; set; }
        public int Demand { get; set; }
    }
}
