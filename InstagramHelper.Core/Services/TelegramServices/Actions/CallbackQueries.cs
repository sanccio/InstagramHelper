using InstagramHelper.Core.Enums;
using InstagramHelper.Core.Models;
using InstagramHelper.Core.Services.InstagramServices.Ig;
using InstagramHelper.Core.Services.SubscriptionsService;
using InstagramHelper.Core.Services.TelegramServices.Handlers;
using InstagramHelper.Core.Services.TelegramServices.States;
using InstagramHelper.Core.Services.TelegramServices.UserService;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace InstagramHelper.Core.Services.TelegramServices.Actions
{
    public class CallbackQueries
    {
        private readonly ITelegramBotClient _botClient;
        private readonly InstaUserDataHandler _instaUserDataHandler;
        private readonly ITelegramUserService _tgUserService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IIgService _igService;
        private readonly ILogger<CallbackQueries> _logger;
        private readonly BotContext _botContext;

        public CallbackQueries(
            ITelegramBotClient botClient,
            InstaUserDataHandler instaUserDataHandler,
            ITelegramUserService tgUserService,
            ISubscriptionService subscriptionService,
            IIgService igService,
            ILogger<CallbackQueries> logger,
            BotContext botContext)
        {
            _botClient = botClient;
            _instaUserDataHandler = instaUserDataHandler;
            _tgUserService = tgUserService;
            _subscriptionService = subscriptionService;
            _igService = igService;
            _logger = logger;
            _botContext = botContext;
        }


        public async Task GetStoriesAsync(long chatId, string instaUsername, CancellationToken cancellationToken)
        {
            await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: BotResponse.WaitOperationEnding,
                    cancellationToken: cancellationToken);

            IEnumerable<Story> stories = await _igService.GetUserStoriesAsync(instaUsername);

            if (!stories.Any())
            {
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: BotResponse.NoStoriesFound,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("No stories found for '@{InstaUsername}'.", instaUsername);
            }

            await _instaUserDataHandler.SendUserStoriesAsAlbumAsync(chatId, stories, cancellationToken);
        }


        public async Task SubscribeAsync(long chatId, string instaUsername, CancellationToken cancellationToken)
        {
            bool isSubscribed = _subscriptionService.IsUserSubscribed(chatId, instaUsername);

            if (isSubscribed)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: BotResponse.UserIsSubscribed,
                    cancellationToken: cancellationToken);
                return;
            }

            await _igService.CreateUserIfNotExistsAsync(new InstaUser { Username = instaUsername });

            _botContext.InstaUsername = instaUsername;
            _botContext.TelegramUser.State = State.WaitingForTimeInput;

            await _tgUserService.UpdateUser(_botContext.TelegramUser);

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: BotResponse.CreateTimeRequestText(instaUsername),
                cancellationToken: cancellationToken);
        }


        public async Task UnsubscribeAsync(long chatId, string instaUsername, CancellationToken cancellationToken)
        {
            bool isSuccess = await _subscriptionService.Unsubscribe(chatId, instaUsername);

            string botResponse;

            if (isSuccess)
            {
                await _igService.DeleteUserAsync(instaUsername);
                botResponse = BotResponse.UnsubscribeSuccess;
            }
            else
            {
                botResponse = BotResponse.UserIsNotSubscribed;
            }

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: botResponse,
                cancellationToken: cancellationToken);

            _logger.LogInformation("User '{ChatId}' unsubscribed from '@{InstaUsername}' stories.", chatId, instaUsername);
        }
    }
}