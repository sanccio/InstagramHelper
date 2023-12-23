using InstagramHelper.Core.Enums;
using InstagramHelper.Core.Services.SubscriptionsService;
using InstagramHelper.Core.Services.TelegramServices.UserService;
using InstagramHelper.Core.Services.TelegramServices.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace InstagramHelper.Core.Services.TelegramServices.States
{
    public class TimeInputState : StateHandler
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ITelegramUserService _tgUserService;
        private readonly ITelegramBotClient _botClient;
        private readonly ILogger<TimeInputState> _logger;

        public TimeInputState(
            ISubscriptionService subscriptionService,
            ITelegramUserService tgUserService,
            ITelegramBotClient botClient,
            ILogger<TimeInputState> logger)
        {
            _subscriptionService = subscriptionService;
            _tgUserService = tgUserService;
            _botClient = botClient;
            _logger = logger;
        }

        public override async Task HandleState(BotContext botContext, Update update, CancellationToken cancellationToken)
        {
            if (botContext.TelegramUser.State != State.WaitingForTimeInput || botContext.InstaUsername == null)
            {
                botContext.TelegramUser.State = State.Empty;
                await _tgUserService.UpdateUser(botContext.TelegramUser);

                throw new Exception("Incorrect state.");
            }

            if (update.Message?.Text == null)
                return;

            bool isTimeValid = TimeOnly.TryParse(update.Message.Text, out TimeOnly parsedTime);

            if (!isTimeValid)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: update.GetChatId(),
                    text: BotResponse.IncorrectTimeFormat,
                    cancellationToken: cancellationToken);
                return;
            }

            await _subscriptionService.SubscribeToInstaUserAsync(update.GetChatId(), botContext.InstaUsername, parsedTime);

            _logger.LogInformation("User '{UserId}' subscribed to '@{InstaUsername}' stories.", botContext.TelegramUser.Id, botContext.InstaUsername);

            await _botClient.SendTextMessageAsync(
                chatId: update.GetChatId(),
                text: BotResponse.SubscribeSuccess,
                cancellationToken: cancellationToken);

            botContext.TelegramUser.State = State.Empty;
            await _tgUserService.UpdateUser(botContext.TelegramUser);
        }
    }
}
