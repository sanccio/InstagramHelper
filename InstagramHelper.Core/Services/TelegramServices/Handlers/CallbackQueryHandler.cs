using InstagramHelper.Core.Services.TelegramServices.Actions;
using InstagramHelper.Core.Services.TelegramServices.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace InstagramHelper.Core.Services.TelegramServices.Handlers
{
    public class CallbackQueryHandler
    {
        private readonly CallbackQueries _callbackQueries;
        private readonly ILogger<CallbackQueryHandler> _logger;

        public CallbackQueryHandler(CallbackQueries callbackQueries, ILogger<CallbackQueryHandler> logger)
        {
            _callbackQueries = callbackQueries;
            _logger = logger;
        }

        public async Task HandleCallbackQueryAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery is not { } callback) return;
            if (callback.Data is not { } callbackData) return;
            if (callback.Message is not { } callbackMessage) return;
            if (string.IsNullOrEmpty(callbackMessage.Text)) return;

            string instaUsername = InstaUsernameUtils.ExtractUsernameAfterSymbol(callbackMessage.Text, '@');

            long chatId = callbackMessage.Chat.Id;

            _logger.LogInformation("Received inline keyboard callback '{CallbackData}' in chat {ChatId}.", callbackData, chatId);

            var action = callbackData switch
            {
                "get_stories" => _callbackQueries.GetStoriesAsync(chatId, instaUsername, cancellationToken),
                "subscribe"   => _callbackQueries.SubscribeAsync(chatId, instaUsername, cancellationToken),
                "unsubscribe" => _callbackQueries.UnsubscribeAsync(chatId, instaUsername, cancellationToken),
                _             => throw new ArgumentOutOfRangeException("Non-existent callback data.", nameof(callbackData))
            };
            await action;
        }
    }
}
