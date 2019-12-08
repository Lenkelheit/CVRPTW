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
        private readonly ISubscription subscription;
        private readonly IBroker broker;

        // CONSTRUCTORS
        public Consumer(IConnectionFactory connectionFactory, Settings settings)
        {
            this.broker = new Broker(connectionFactory, settings);                        

            this.subscription = new Subscription(broker.Channel, settings.QueueName, autoAck: false);
        }
        public void Dispose()
        {
            broker?.Dispose();
            subscription?.Dispose();
        }

        // METHODS
        public void SetAcknowledge(ulong deliveryTag, bool processed)
        {
            if (processed)
            {
                broker.Channel.BasicAck(deliveryTag, false);
            }
            else
            {
                broker.Channel.BasicNack(deliveryTag, multiple: false, requeue: true);
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