using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeaLiveAuction.Hubs
{
    public class AuctionHub : Hub
    {
        public async Task SendHighestBid()
        {
            await Clients.All.SendAsync("ReceiveHBPUpdate");
        }
    }
}
