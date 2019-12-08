using System.Collections.Generic;

namespace Domains.Models.Output
{
    public class FileOutput
    {
        public IList<Summary> Summaries { get; set; }
        public IList<Itineraries> Itineraries { get; set; } 
        public IList<Totals> Totals { get; set; } 
        public IList<Dropped> DroppedLocation { get; set; } 
    }
}
