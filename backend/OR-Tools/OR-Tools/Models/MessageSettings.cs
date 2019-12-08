using QueueService.Models;

namespace OR_Tools.Models
{
    internal class MessageSettings
    {
        public Settings InputData { get; set; }
        public Settings Result { get; set; }
        public Settings IsSolved { get; set; }
    }
}
