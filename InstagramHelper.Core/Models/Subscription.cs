using System.ComponentModel.DataAnnotations.Schema;

namespace InstagramHelper.Core.Models
{
    public class Subscription
    {
        [ForeignKey("InstaUser")]
        public string InstaUsername { get; set; } = default!;

        public InstaUser InstaUser { get; set; } = default!;

        public long TelegramUserId { get; set; }

        public TelegramUser TelegramUser { get; set; } = default!;
    }
}
