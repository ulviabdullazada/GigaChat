namespace GigaChat.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string RoomName { get; set; } = "global";
        public string ConnectionId { get; set; }
    }
}
