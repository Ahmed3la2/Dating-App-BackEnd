using BackEnd.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace BackEnd.signalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tacker;

        public PresenceHub(PresenceTracker tacker)
        {
            _tacker = tacker;
        }

        public override async Task OnConnectedAsync()
        {
            await _tacker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            var useronline = await _tacker.GetUsersOnline();
            await Clients.All.SendAsync("GetOnlineUsers", useronline);
        }
        
        public override async Task  OnDisconnectedAsync(Exception exception)
        {
            await _tacker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);

            await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
            var useronline = await _tacker.GetUsersOnline();
            await Clients.All.SendAsync("GetOnlineUsers", useronline);

            await base.OnDisconnectedAsync(exception);
        }

    }
}
