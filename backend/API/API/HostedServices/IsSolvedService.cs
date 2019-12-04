using Domains.Models.Output;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using QueueService.Interfaces;

using System.Threading;
using System.Threading.Tasks;

namespace API.HostedServices
{
    public class IsSolvedService : BackgroundService
    {
        IHubContext<Hubs.OrToolsHub> hubContext;
        IConsumer consumer;

        public IsSolvedService(IHubContext<Hubs.OrToolsHub> hubContext, IConnectionProvider factory, IConfiguration configuration)
        {
            QueueService.Models.Settings settings = new QueueService.Models.Settings();
            configuration.Bind("RabbitMq:IsSolved", settings);

            this.consumer = factory.Connect(settings);

            this.hubContext = hubContext;
        }

        public override void Dispose()
        {
            base.Dispose();
            consumer.Dispose();
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                QueueService.Models.ReceiveData receiveData = await Task.Run(() => consumer.Receive(2500));

                if (receiveData != null)
                {
                    bool isSolved = receiveData.GetObject<bool>();
                    await hubContext.Clients.All.SendAsync("IsSolved", isSolved);
                    consumer.SetAcknowledge(receiveData.DeliveryTag, processed: true);
                }
            }
        }
    }
}
