using OR_Tools.Models;
using OR_Tools.Solvers;
using OR_Tools.Interfaces;

using QueueService.Interfaces;
using QueueService.QueueServices;

using System;
using System.Threading;

using Microsoft.Extensions.Configuration;

using RabbitMQ.Client;

namespace OR_Tools
{
    internal class Program
    {
        private static readonly int MAX_REQUEST_TIMEOUT = 2500;

        public static void Main(string[] args)
        {
            IConfiguration configuration = GetConfiguration();
            IConnectionProvider connectionProvider = GetConnectionProvider(configuration);
            MessageSettings messageSettings = GetSettings(configuration);
            ISolver solver = GetSolver();
            CancellationToken cancellationToken = new CancellationToken();

            using (Bootstrapper bootstrapper = new Bootstrapper(connectionProvider, messageSettings, solver))
            {
                bootstrapper.Run(cancellationToken, MAX_REQUEST_TIMEOUT);
            }
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables()
                        .Build();
        }

        private static MessageSettings GetSettings(IConfiguration configuration)
        {
            return configuration.GetSection("RabbitMq").Get<MessageSettings>();
        }

        private static IConnectionProvider GetConnectionProvider(IConfiguration configuration)
        {
            string hostName = configuration.GetValue<string>("RabbitMq:HostName");
            IConnectionFactory connectionFactory = new DefaultConnectionFactory(hostName);
            return new ConnectionProvider(connectionFactory);
        }
        private static ISolver GetSolver()
        {
            return new OrToolsSolver();
        }
    }
}
