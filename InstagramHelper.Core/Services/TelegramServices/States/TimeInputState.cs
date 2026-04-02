using InstagramHelper.Core.Enums;
using InstagramHelper.Core.Services.SubscriptionsService;
using InstagramHelper.Core.Services.TelegramServices.UserService;
using InstagramHelper.Core.Services.TelegramServices.Utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace InstagramHelper.Core.Services.TelegramServices.States
{
    public class TimeInputState : StateHandler
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ITelegramUserService _tgUserService;
        private readonly ITelegramBotClient _botClient;

        public TimeInputState(
            ISubscriptionService subscriptionService,
            ITelegramUserService tgUserService,
            ITelegramBotClient botClient)
        {
            _subscriptionService = subscriptionService;
            _tgUserService = tgUserService;
            _botClient = botClient;
        }

        public override async Task HandleState(BotContext botContext, Update update, CancellationToken cancellationToken)
        {
            await EnsureValidState(botContext);

            if (update.Message?.Text is not { } messageText)
                return;

            long chatId = update.GetChatId();

            bool isTimeValid = TimeOnly.TryParse(messageText, out TimeOnly parsedTime);

            if (!isTimeValid)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: BotResponse.IncorrectTimeFormat,
                    cancellationToken: cancellationToken);
                return;
            }

            await _subscriptionService.SubscribeToInstaUserAsync(chatId, botContext.InstaUser!, parsedTime);

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: BotResponse.SubscribeSuccess,
                cancellationToken: cancellationToken);

            await ResetStateAsync(botContext);
        }


        private async Task EnsureValidState(BotContext botContext)
        {
            State currentState = botContext.TelegramUser.State;

            if (currentState != State.WaitingForTimeInput
                || botContext.InstaUser?.Username == null)
            {
                await ResetStateAsync(botContext);

                throw new Exception($"Incorrect state: {currentState}. State was reset to Empty.");
            }
        }


        private async Task ResetStateAsync(BotContext botContext)
        {
            botContext.TelegramUser.State = State.Empty;
            await _tgUserService.UpdateUser(botContext.TelegramUser);
        }
    }
}
