using Microsoft.AspNetCore.SignalR;
using TeaLiveAuction.Hubs;

namespace AuctionService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHubContext<AuctionHub> _hubContext;

        public Worker(ILogger<Worker> logger, IHubContext<AuctionHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(10000, stoppingToken);
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await _hubContext.Clients.All.SendAsync("ReceiveHBPUpdate");
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
