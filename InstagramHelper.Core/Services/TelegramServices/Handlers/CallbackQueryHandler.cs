using InstagramHelper.Core.Models;
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

            (string action, long? instaUserId) = ParseCallbackData(callbackData);

            var instaUser = new IgUserIdentifier(instaUsername, instaUserId);

            var task = action switch
            {
                "get_stories" => _callbackQueries.GetStoriesAsync(chatId, instaUser, cancellationToken),
                "subscribe"   => _callbackQueries.SubscribeAsync(chatId, instaUser, cancellationToken),
                "unsubscribe" => _callbackQueries.UnsubscribeAsync(chatId, instaUsername, cancellationToken),
                _             => throw new InvalidOperationException($"Unknown callback action: {action}.")
            };
            await task;
        }


        private static (string action, long? instaUserId) ParseCallbackData(string callbackData)
        {
            string[] parts = callbackData.Split(':');
            
            string action = parts[0];

            if (parts.Length > 1 && long.TryParse(parts[1], out long instaUserId))
            {
                return (action, instaUserId);
            }

            return (action, null);
        }
    }
}
