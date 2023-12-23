using InstagramHelper.Core.Services.TelegramServices.Actions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace InstagramHelper.Core.Services.TelegramServices.Handlers
{
    public class MessageHandler
    {
        private readonly InstaUserDataHandler _instaUserDataHandler;
        private readonly Commands _commands;
        private readonly ILogger<InstaUserDataHandler> _logger;

        public MessageHandler(InstaUserDataHandler instaUserDataHandler, Commands commands, ILogger<InstaUserDataHandler> logger)
        {
            _instaUserDataHandler = instaUserDataHandler;
            _commands = commands;
            _logger = logger;
        }

        public async Task HandleMessageAsync(Message message, CancellationToken cancellationToken)
        {
            if (message is null || message.Text is not { } messageText)
                return;

            long chatId = message.Chat.Id;

            _logger.LogInformation("Received '{Text}' message in chat {ChatId}.", messageText, chatId);

            if (messageText.StartsWith("/"))
            {
                var action = messageText switch
                {
                    "/start"         => _commands.HandleStartCommandAsync(chatId, cancellationToken),
                    "/subscriptions" => _commands.HandleSubscriptionsCommandAsync(chatId, cancellationToken),
                    "/info"          => _commands.HandleInfoCommandAsync(chatId, cancellationToken),
                    _                => _commands.HandleUnknownCommandAsync(chatId, cancellationToken),
                };
                await action;

                return;
            }

            await _instaUserDataHandler.SendInstagramUserInfoAsync(messageText, chatId, cancellationToken);
        }
    }
}