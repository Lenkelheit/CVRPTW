using RabbitMQ.Client;
using QueueService.Interfaces;

namespace QueueService.QueueServices
{
    public class ConnectionProvider : IConnectionProvider
    {
        // FIELDS
        private readonly IConnectionFactory connectionFactory;

        // CONSTRUCTORS
        public ConnectionProvider(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }
        
        // METHODS
        public IConsumer Connect(Models.Settings settings)
        {
            return new Consumer(connectionFactory, settings);
        }
        public IProducer Open(Models.Settings settings)
        {
            return new Producer(connectionFactory, settings);
        }
    }
}
