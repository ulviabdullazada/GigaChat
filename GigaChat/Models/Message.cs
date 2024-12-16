namespace GigaChat.Models
{
    public class Message
    {
        public UserAndImage User { get; set; }
        public string Content { get; set; }
        public string SendDate { get; set; }
    }
}
