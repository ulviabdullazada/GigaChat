using GigaChat.Models;

namespace GigaChat
{
    public class Helper
    {
        public static ICollection<User> Users { get; } = new HashSet<User>();
    }
}
