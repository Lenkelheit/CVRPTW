namespace QueueService.QueueServices
{
    public class DefaultConnectionFactory : RabbitMQ.Client.ConnectionFactory
    {
        public DefaultConnectionFactory(string hostName = "localhost")
        {
            UserName = "guest";
            Password = "guest";
            VirtualHost = "/";
            HostName = hostName;
        }
    }
}
