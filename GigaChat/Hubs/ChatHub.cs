using GigaChat.Models;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace GigaChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task JoinRoom(string room)
        {
            var user = _getCurrentUser();
            user!.RoomName = room;
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
            await _sendUsers(user, room);
        }
        public async Task SendMessage(string message)
        {
            message = message.Trim();
            if (!string.IsNullOrWhiteSpace(message))
            {
                var user = _getCurrentUser()!;
                var msg = new Message
                {
                    Content = message,
                    SendDate = DateTime.Now.ToShortTimeString(),
                    User = new()
                    {
                        Image = user.Image,
                        Name = user.Name
                    }
                };
                await Clients.Groups(user.RoomName).SendAsync("RecieveMessage", msg);
            }
        }
        public override async Task OnConnectedAsync()
        {
            if (Context.User != null)
            {
                var claims = Context.User.Claims.ToList();
                var user = Helper.Users.FirstOrDefault(x => x.Name == claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value);
                if (user != null)
                {
                    user.ConnectionId = Context.ConnectionId;
                    await _sendUsers(user);
                }
                else if (claims.Any())
                {
                    Helper.Users.Add(new User
                    {
                        ConnectionId = Context.ConnectionId,
                        Name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value,
                        Id = Guid.NewGuid(),
                        RoomName = "global",
                        Image = claims.FirstOrDefault(x => x.Type == "image").Value
                    });
                }
            }
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = _getCurrentUser();
            Helper.Users.Remove(user);
            await _sendUsers(user);
            await base.OnDisconnectedAsync(exception);
        }
        async Task _sendUsers(User? user, string roomName)
        {
            //Userl-lari ve shekilleri gotur
            var usersAndImages = Helper.Users
                .Where(x => x.ConnectionId != null && x.RoomName == roomName)
                .Select(u => new UserAndImage
            {
                Image = u.Image,
                Name = u.Name
            });
            await Clients.Group(roomName).SendAsync("newUser", usersAndImages);
        }
        async Task _sendUsers(User? user)
        {
            //Userl-lari ve shekilleri gotur
            var usersAndImages = Helper.Users.Where(x => x.ConnectionId != null).Select(u => new UserAndImage
            {
                Image = u.Image,
                Name = u.Name
            });
            await Clients.All.SendAsync("newUser", usersAndImages);
            
        }
        User? _getCurrentUser()
        {
            var sad = Context.ConnectionId;
            var user = Helper.Users.FirstOrDefault(x => x.ConnectionId == sad);
            return user;
        }
    }
}
