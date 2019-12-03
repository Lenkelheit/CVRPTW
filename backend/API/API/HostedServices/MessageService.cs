using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

using System.Threading;
using System.Threading.Tasks;

namespace API.HostedServices
{
    public class MessageService : BackgroundService
    {
        IHubContext<Hubs.MessageHub> hubContext;

        public MessageService(IHubContext<Hubs.MessageHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000);
                await hubContext.Clients.All.SendAsync("Solved", true);
            }
        }
    }
}
