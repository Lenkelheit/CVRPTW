using System.ComponentModel;

namespace Domains.Models.Output
{
    public class Totals
    {
        [DisplayName("Total loads")]
        public int Load { get; set; }
        [DisplayName("Total distance")]
        public int Distance { get; set; }
        [DisplayName("Total travel minutes")]
        public int Time { get; set; }
    }
}
