using System;

namespace Domains.Models.Input
{
    public class Orders
    {
        public int Id { get; set; }
        public int Volume { get; set; }
        public int Weight { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
