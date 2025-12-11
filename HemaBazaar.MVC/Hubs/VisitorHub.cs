using Microsoft.AspNetCore.SignalR;

namespace HemaBazaar.MVC.Hubs
{
    public class VisitorHub : Hub
    {
        static int _visitorCount = 0;
        public override async Task OnConnectedAsync()
        {
            _visitorCount++;
            await Clients.All.SendAsync("ReceiveVisitorCount", _visitorCount);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _visitorCount--;
            await Clients.All.SendAsync("ReceiveVisitorCount", _visitorCount);
            await base.OnDisconnectedAsync(exception);
        }


    }
}
