using InstagramHelper.Core.Enums;
using InstagramHelper.Core.Models;
using InstagramHelper.Core.Services.InstagramServices.Ig;
using InstagramHelper.Core.Services.SubscriptionsService;
using InstagramHelper.Core.Services.TelegramServices.Keyboards;
using InstagramHelper.Core.Services.TelegramServices.UserService;
using InstagramHelper.Core.Services.TelegramServices.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RetryExecutor _retryExecutor;
        private readonly ITelegramUserService _tgUserService;

        public InstaUserDataHandler(
            ITelegramBotClient botClient,
            IIgService igService,
            ISubscriptionService subscriptionService,
            ILogger<InstaUserDataHandler> logger,
            IHttpClientFactory httpClientFactory,
            RetryExecutor retryExecutor,
            ITelegramUserService tgUserService)
        {
            _botClient = botClient;
            _igService = igService;
            _subscriptionService = subscriptionService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _retryExecutor = retryExecutor;
            _tgUserService = tgUserService;
        }


        public async Task<IAlbumInputMedia[]> CreateStoryAlbumAsync(Story[] stories)
        {
            var album = new IAlbumInputMedia[stories.Length];

            HttpClient httpClient = _httpClientFactory.CreateClient("InstaUserDataHandler");

            for (int i = 0; i < stories.Length; i++)
            {
                if (stories[i].VideoVersions.Count == 0)
                {
                    album[i] = new InputMediaPhoto(InputFile.FromUri(stories[i].ImageVersions.Candidates[1].Url))
                    {
                        Caption = _igService.GetMediaPassedTime(stories[i].TakenAt)
                    };
                }
                else if (stories[i].VideoVersions.Count > 0)
                {
                    Stream stream = await _retryExecutor
                        .Retry(() => httpClient.GetStreamAsync(stories[i].VideoVersions[0].Url));

                    album[i] = new InputMediaVideo(InputFile.FromStream(stream, $"{Guid.NewGuid()}.mp4"))
                    {
                        SupportsStreaming = true,
                        Caption = _igService.GetMediaPassedTime(stories[i].TakenAt)
                    };
                }
            }
            return album;
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
                media: stories,
                inputMediaCreator: CreateStoryAlbumAsync,
                cancellationToken: cancellationToken);

            _logger.LogInformation("Sent '{Count}' stories to '{ChatId}'.", stories.Count(), chatId);
        }
    }
}
