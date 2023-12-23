using InstagramHelper.Core.Models;

namespace InstagramHelper.Core.Services.TelegramServices.States
{
    public class BotContext
    {
        public TelegramUser TelegramUser { get; set; }

        public string? InstaUsername { get; set; }
    }
}
