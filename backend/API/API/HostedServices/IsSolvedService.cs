using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

using System.Threading;
using System.Threading.Tasks;

namespace API.HostedServices
{
    public class IsSolvedService : BackgroundService
    {
        IHubContext<Hubs.OrToolsHub> hubContext;

        public IsSolvedService(IHubContext<Hubs.OrToolsHub> hubContext)
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
