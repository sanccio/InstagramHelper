using InstagramHelper.Core.Models;
using InstagramHelper.Core.Services.InstagramServices.Ig;
using InstagramHelper.Core.Services.TelegramServices;
using Microsoft.Extensions.Logging;
using Quartz;

namespace InstagramHelper.Core.Services.SchedulerService
{
    public class StoriesSender : IJob
    {
        private readonly IIgService _igService;
        private readonly InstaUserDataHandler _instaUserDataHandler;
        private readonly ILogger<StoriesSender> _logger;

        public StoriesSender(
            IIgService igService,
            InstaUserDataHandler instaUserDataHandler,
            ILogger<StoriesSender> logger)
        {
            _igService = igService;
            _instaUserDataHandler = instaUserDataHandler;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobDataMap = context.MergedJobDataMap;

            long? instaUserPk = jobDataMap.ContainsKey(SchedulerKeys.InstagramUserPkKey)
                ? jobDataMap.GetLong(SchedulerKeys.InstagramUserPkKey)
                : null;

            string? instaUsername = jobDataMap.GetString(SchedulerKeys.InstagramUsernameKey);

            if (string.IsNullOrEmpty(instaUsername))
            {
                throw new JobExecutionException($"Required parameter '{instaUsername}' not found in JobDataMap");
            }

            long chatId = jobDataMap.GetLong(SchedulerKeys.TelegramUserIdKey);

            if (chatId == 0)
            {
                throw new JobExecutionException($"Required parameter '{chatId}' not found in JobDataMap");
            }

            _logger.LogInformation(
                "Executing StoriesSender for '@{InstagramUsername}' (pk:{Pk}) to chat '{ChatId}'.",
                instaUsername,
                instaUserPk,
                chatId);

            var stories = await _igService.GetUserStoriesAsync(new IgUserIdentifier(instaUsername, instaUserPk));

            await _instaUserDataHandler.SendUserStoriesAsAlbumAsync(chatId, stories);
        }
    }
}
