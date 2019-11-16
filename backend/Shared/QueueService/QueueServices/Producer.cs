using QueueService.Interfaces;
using QueueService.Models;

using RabbitMQ.Client;

namespace QueueService.QueueServices
{
    public class Producer : IProducer
    {
        // FIELDS
        private readonly PublicationAddress publicationAddress;

        private IBroker connectionLine;

        // CONSTRUCTORS
        public Producer(IConnectionFactory connectionFactory, Settings settings)
        {
            this.connectionLine = new Broker(connectionFactory, settings);            

            this.publicationAddress = new PublicationAddress(
                    settings.ExchangeType,
                    settings.ExchangeName,
                    settings.RoutingKey);
        }
        
        public void Dispose()
        {
            connectionLine?.Dispose();
        }

        // PROPERTIES
        public IModel Channel => connectionLine.Channel;

        // METHODS
        public void Send(string message)
        {
            byte[] body = System.Text.Encoding.UTF8.GetBytes(message);

            Channel.BasicPublish(publicationAddress, null, body);
        }

    }
}