namespace QueueService.Interfaces
{
    internal interface IBroker : System.IDisposable
    {
        RabbitMQ.Client.IModel Channel { get; }
        
        void DeclareExchange(string exchangeName, string exchangeType);
        void BindQueue(string exchangeName, string queueName, string routingKey);
    }
}
