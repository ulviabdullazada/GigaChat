using GigaChat.Models;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GigaChat.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            if (Context.User != null)
            {
                var claims = Context.User.Claims.ToList();
                var user = Helper.Users.FirstOrDefault(x => x.Name == claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value);
                if (user != null)
                {
                    user.ConnectionId = Context.ConnectionId;
                    //Userl-lari ve shekilleri gotur
                    var usersAndImages = Helper.Users.Where(x => x.ConnectionId != null).Select(u => new UserAndImage
                    {
                        Image = u.Image,
                        Name = u.Name
                    });
                    Clients.All.SendAsync("newUser", usersAndImages);
                }
                

            }
            return base.OnConnectedAsync();
        }
    }
}
