using InstagramHelper.Core.Services.SubscriptionsService;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace InstagramHelper.Core.Services.TelegramServices.Actions
{
    public class Commands
    {
        private readonly ITelegramBotClient _botClient;
        private readonly ISubscriptionService _subscriptionService;

        public Commands(ITelegramBotClient botClient, ISubscriptionService subscriptionService)
        {
            _botClient = botClient;
            _subscriptionService = subscriptionService;
        }


        public async Task HandleSubscriptionsCommandAsync(long chatId, CancellationToken cancellationToken)
        {
            await _botClient.SendChatActionAsync(chatId, ChatAction.Typing, cancellationToken: cancellationToken);

            string[] usernames = await _subscriptionService.GetAllSubscriptionUsernamesAsync(chatId);
            string usernamesList = BotResponse.CreateSubscriptionsListText(usernames);

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: usernamesList,
                cancellationToken: cancellationToken);
        }


        public async Task HandleStartCommandAsync(long chatId, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: BotResponse.StartCommand,
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
        }


        public async Task HandleInfoCommandAsync(long chatId, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(chatId, BotResponse.BotInfoCommand, cancellationToken: cancellationToken);
        }


        public async Task HandleUnknownCommandAsync(long chatId, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(chatId, BotResponse.UnknownCommand, cancellationToken: cancellationToken);
        }
    }
}