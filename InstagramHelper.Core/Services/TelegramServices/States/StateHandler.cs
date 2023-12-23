using Telegram.Bot.Types;

namespace InstagramHelper.Core.Services.TelegramServices.States
{
    abstract public class StateHandler
    {
        public abstract Task HandleState(BotContext botContext, Update update, CancellationToken cancellationToken);
    }
}
