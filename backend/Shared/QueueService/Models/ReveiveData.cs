using Newtonsoft.Json;

namespace QueueService.Models
{
    /// <summary>
    /// The data that Consumer receive
    /// </summary>
    public class ReceiveData
    {
        public ulong DeliveryTag { get; set; }
        public string Message { get; set; }

        public TModel GetObject<TModel>()
        {
            return JsonConvert.DeserializeObject<TModel>(Message);
        }
    }
}
