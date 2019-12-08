using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using QueueService.Models;
using QueueService.Interfaces;

using System.Threading;
using System.Threading.Tasks;

namespace API.HostedServices
{
    public class IsSolvedService : BackgroundService
    {
        private readonly IHubContext<Hubs.OrToolsHub> hubContext;
        private readonly IConsumer consumer;

        public IsSolvedService(IHubContext<Hubs.OrToolsHub> hubContext, IConnectionProvider factory, IOptionsMonitor<Settings> options)
        {
            this.consumer = factory.Connect(options.Get("IsSolvedQueue"));
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
                ReceiveData receiveData = await Task.Run(() => consumer.Receive(2500));

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
