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

        public StoriesSender(IIgService igService, InstaUserDataHandler instaUserDataHandler, ILogger<StoriesSender> logger)
        {
            _igService = igService;
            _instaUserDataHandler = instaUserDataHandler;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobDataMap = context.MergedJobDataMap;

            string? instaUserId = jobDataMap.GetString(SchedulerKeys.InstagramUserIdKey);

            if (string.IsNullOrEmpty(instaUserId))
            {
                throw new JobExecutionException($"Required parameter '{instaUserId}' not found in JobDataMap");
            }

            long chatId = jobDataMap.GetLong(SchedulerKeys.TelegramUserIdKey);

            if (chatId == 0)
            {
                throw new JobExecutionException($"Required parameter '{chatId}' not found in JobDataMap");
            }

            _logger.LogInformation("Executing 'SendingStoriesJob'. Trying to send stories i:{InstagramUserId} -> tg:{ChatId}'.", instaUserId, chatId);

            IEnumerable<Story> stories = await _igService.GetUserStoriesAsync(instaUserId);

            await _instaUserDataHandler.SendUserStoriesAsAlbumAsync(chatId, stories);
        }
    }
}
