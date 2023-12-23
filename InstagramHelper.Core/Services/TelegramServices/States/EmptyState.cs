using InstagramHelper.Core.Services.TelegramServices.Handlers;
using InstagramHelper.Core.Services.TelegramServices.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InstagramHelper.Core.Services.TelegramServices.States
{
    public class EmptyState : StateHandler
    {
        private readonly CallbackQueryHandler _callbackQueryHandler;
        private readonly MessageHandler _messageHandler;
        private readonly ILogger<StateHandler> _logger;

        public EmptyState(
            CallbackQueryHandler callbackQueryHandler,
            MessageHandler messageHandler,
            ILogger<StateHandler> logger)
        {
            _callbackQueryHandler = callbackQueryHandler;
            _messageHandler = messageHandler;
            _logger = logger;
        }

        public override async Task HandleState(BotContext botContext, Update update, CancellationToken cancellationToken)
        {
            var action = update.Type switch
            {
                UpdateType.Message       => _messageHandler.HandleMessageAsync(update.Message!, cancellationToken),
                UpdateType.CallbackQuery => _callbackQueryHandler.HandleCallbackQueryAsync(update.CallbackQuery!, cancellationToken),
                UpdateType.MyChatMember  => Task.Run(() => _logger.LogWarning("Bot was blocked by the user '{userId}'.", update.GetChatId()), cancellationToken),
                _                        => throw new ArgumentOutOfRangeException("Non-existent update type.", nameof(update.Type)),
            };
            await action;
        }
    }
}
