using QueueService.Models;
using QueueService.Interfaces;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace QueueService.QueueServices
{
    public class Consumer : IConsumer
    {
        // FIELDS        
        private ISubscription subscription;
        private IBroker connectionLine;

        // CONSTRUCTORS
        public Consumer(IConnectionFactory connectionFactory, Settings settings)
        {
            this.connectionLine = new Broker(connectionFactory, settings);                        

            this.subscription = new Subscription(connectionLine.Channel, settings.QueueName, autoAck: false);
        }
        public void Dispose()
        {
            connectionLine?.Dispose();
            subscription?.Dispose();
        }

        // METHODS
        public void SetAcknowledge(ulong deliveryTag, bool processed)
        {
            if (processed)
            {
                connectionLine.Channel.BasicAck(deliveryTag, false);
            }
            else
            {
                connectionLine.Channel.BasicNack(deliveryTag, multiple: false, requeue: true);
            }
        }

        public ReceiveData Receive(int millisecondsTimeout)
        {
            if (subscription.Next(millisecondsTimeout, out BasicDeliverEventArgs basicDeliveryEventArgs))
            {
                return new ReceiveData
                {
                    DeliveryTag = basicDeliveryEventArgs.DeliveryTag,
                    Message = System.Text.Encoding.UTF8.GetString(basicDeliveryEventArgs.Body)
                };
            }
            else return null;            
        }
    }
}