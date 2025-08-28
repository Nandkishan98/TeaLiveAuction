using AuctionService;
using Microsoft.AspNetCore.SignalR;
using TeaLiveAuction.Hubs;

var builder = Host.CreateApplicationBuilder(args);
//builder.Services.AddHostedService<Worker>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<Worker>(x =>
{
    //var connectionString = configuration.GetConnectionString("TeaboardConn");
    var logger = x.GetRequiredService<ILogger<Worker>>();
    var hubContext = x.GetRequiredService<IHubContext<AuctionHub>>();
    return new Worker(logger, hubContext);
});

var host = builder.Build();
host.Run();
