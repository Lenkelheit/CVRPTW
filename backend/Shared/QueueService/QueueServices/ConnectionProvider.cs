using RabbitMQ.Client;

namespace QueueService.QueueServices
{
    public class ConnectionProvider : Interfaces.IConnectionProvider
    {
        // FIELDS
        private readonly IConnectionFactory connectionFactory;

        // CONSTRUCTORS
        public ConnectionProvider(IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }
        
        // METHODS
        public Interfaces.IConsumer Connect(Models.Settings settings)
        {
            return new Consumer(connectionFactory, settings);
        }
        public Interfaces.IProducer Open(Models.Settings settings)
        {
            return new Producer(connectionFactory, settings);
        }
    }
}
