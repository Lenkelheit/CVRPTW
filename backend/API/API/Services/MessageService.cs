using Domains.Models.Input;
using Domains.Models.Output;
using Microsoft.Extensions.Configuration;
using QueueService.Interfaces;
using QueueService.Models;

namespace API.Services
{
    public class MessageService
    {
        IConnectionProvider factory;
        IConfiguration configuration;

        public MessageService(IConnectionProvider factory, IConfiguration configuration)
        {
            this.factory = factory;
            this.configuration = configuration;
        }

        public void SendFileData(FileInput fileInput)
        {
            Settings settings = new Settings();
            configuration.Bind("RabbitMq:FileData", settings);
            using (IProducer producer = factory.Open(settings))
            {
                producer.Send(fileInput);
            }
        }

        public FileOutput DequeueData()
        {
            Settings settings = new Settings();
            configuration.Bind("RabbitMq:Result", settings);
            using (IConsumer consumer = factory.Connect(settings))
            {
                ReceiveData receiveData = consumer.Receive(500);
                if (receiveData == null) return null;

                consumer.SetAcknowledge(receiveData.DeliveryTag, processed: true);
                return receiveData.GetObject<FileOutput>();
            }
        }
    }
}
