using InstagramHelper.Core.Enums;

namespace InstagramHelper.Core.Models
{
    public class TelegramUser
    {
        public long Id { get; set; }

        public string? Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public AccessLevel AccessLevel { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

        public State State { get; set; }
    }
}
