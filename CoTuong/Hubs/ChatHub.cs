using Microsoft.AspNetCore.SignalR;

namespace CoTuong.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, string roomId)
        {
             
            await Clients.All.SendAsync(roomId, user, message);
            //Clients.Clients()
        }
        public override Task OnConnectedAsync()
        {
            string id = Context.ConnectionId;
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string id = Context.ConnectionId;
            return base.OnDisconnectedAsync(exception);
        }
    }
}
