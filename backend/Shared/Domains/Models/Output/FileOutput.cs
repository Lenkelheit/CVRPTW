using System.Collections.Generic;

namespace Domains.Models.Output
{
    public class FileOutput
    {
        public IList<Summary> Summaries { get; set; }
        public IList<Itineraries> Itineraries { get; set; }
    }
}
