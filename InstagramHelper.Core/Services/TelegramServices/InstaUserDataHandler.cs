using InstagramHelper.Core.Enums;
using InstagramHelper.Core.Models;
using InstagramHelper.Core.Services.InstagramServices.Ig;
using InstagramHelper.Core.Services.SubscriptionsService;
using InstagramHelper.Core.Services.TelegramServices.Keyboards;
using InstagramHelper.Core.Services.TelegramServices.MediaServices;
using InstagramHelper.Core.Services.TelegramServices.UserService;
using InstagramHelper.Core.Services.TelegramServices.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace InstagramHelper.Core.Services.TelegramServices
{
    public class InstaUserDataHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IIgService _igService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<InstaUserDataHandler> _logger;
        private readonly ITelegramUserService _tgUserService;
        private readonly IStoryAlbumComposer _mediaService;

        public InstaUserDataHandler(
            ITelegramBotClient botClient,
            IIgService igService,
            ISubscriptionService subscriptionService,
            ILogger<InstaUserDataHandler> logger,
            ITelegramUserService tgUserService,
            IStoryAlbumComposer mediaService)
        {
            _botClient = botClient;
            _igService = igService;
            _subscriptionService = subscriptionService;
            _logger = logger;
            _tgUserService = tgUserService;
            _mediaService = mediaService;
        }


        public async Task SendInstagramUserInfoAsync(string instaUsername, long chatId, CancellationToken cancellationToken)
        {
            if (await InstaUsernameHelper.ValidateUsername(_botClient, instaUsername, chatId)
                is not { } validatedUsername)
            {
                return;
            }

            await _botClient.SendChatActionAsync(chatId, ChatAction.Typing, cancellationToken: cancellationToken);

            InstaUser? instaUser = await _igService.GetUserAsync(validatedUsername);

            if (instaUser is null)
            {
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: BotResponse.UserNotFound,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("User '{InstaUsername}' not found.", validatedUsername);

                return;
            }

            bool isSubscribed = _subscriptionService.IsUserSubscribed(chatId, instaUser.Username);
            AccessLevel accessLevel = await _tgUserService.GetAccessLevelAsync(chatId);

            InlineKeyboardMarkup inlineKeyboard;

            if (accessLevel == AccessLevel.FullAccess || accessLevel == AccessLevel.Admin)
            {
                inlineKeyboard = UserInfoKeyboard.AttachInlineKeyboardMarkup;
            }
            else
            {
                inlineKeyboard = UserInfoKeyboard.AttachReducedInlineKeyboardMarkup;
            }

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: BotResponse.CreateFullInstaUserInfoText(instaUser, isSubscribed),
                parseMode: ParseMode.Html,
                replyMarkup: instaUser.IsPrivate ? null : inlineKeyboard,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Sent '@{InstaUsername}' info to '{UserId}'.", instaUser.Username, chatId);
        }


        public async Task SendUserStoriesAsAlbumAsync(long chatId, IEnumerable<Story> stories, CancellationToken cancellationToken = default)
        {
            await _botClient.SendOneOrMoreMediaGroupAsync(
                chatId: chatId,
                media: stories.ToArray(),
                inputMediaCreator: _mediaService.CreateStoryAlbumsAsync,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Sent '{Count}' stories to '{ChatId}'.", stories.Count(), chatId);
        }
    }
}
