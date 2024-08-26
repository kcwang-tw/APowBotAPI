namespace TelegramBot.Models
{
    public class ProfileAndContact
    {
        public UserProfile Profile { get; set; } = new();

        public UserContact Contact { get; set; } = new();
    }
}
