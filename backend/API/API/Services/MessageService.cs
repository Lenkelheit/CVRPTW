using Domains.Models.Input;
using Domains.Models.Output;
using Microsoft.Extensions.Options;
using QueueService.Interfaces;
using QueueService.Models;

namespace API.Services
{
    public class MessageService
    {
        private readonly IConnectionProvider factory;
        private readonly Settings postFileSetting;
        private readonly Settings downloadFileSetting;

        public MessageService(IConnectionProvider factory, IOptionsSnapshot<Settings> options)
        {
            this.factory = factory;
            this.postFileSetting = options.Get("PostFileQueue");
            this.downloadFileSetting = options.Get("DownloadFileQueue");
        }

        public void SendFileData(FileInput fileInput)
        {
            using (IProducer producer = factory.Open(postFileSetting))
            {
                producer.Send(fileInput);
            }
        }

        public FileOutput DequeueData()
        {
            using (IConsumer consumer = factory.Connect(downloadFileSetting))
            {
                ReceiveData receiveData = consumer.Receive(500);
                if (receiveData == null) return null;

                consumer.SetAcknowledge(receiveData.DeliveryTag, processed: true);
                return receiveData.GetObject<FileOutput>();
            }
        }
    }
}
