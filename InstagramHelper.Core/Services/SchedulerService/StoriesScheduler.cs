using InstagramHelper.Core.Models;
using Microsoft.Extensions.Logging;
using Quartz;

namespace InstagramHelper.Core.Services.SchedulerService
{
    public class StoriesScheduler
    {
        const string jobPrefix = "SendingStoriesJob_";

        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<StoriesScheduler> _logger;

        public StoriesScheduler(ISchedulerFactory schedulerFactory, ILogger<StoriesScheduler> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }


        public async Task ScheduleStoriesSending(long telegramUserId, IgUserIdentifier igUser, TimeOnly utcTime)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();

            string jobName = $"{jobPrefix}{igUser.Username}";

            var jobBuilder = JobBuilder.Create<StoriesSender>()
                    .WithIdentity(name: jobName,
                                  group: telegramUserId.ToString())
                    .UsingJobData(SchedulerKeys.InstagramUsernameKey, igUser.Username)
                    .UsingJobData(SchedulerKeys.TelegramUserIdKey, telegramUserId);

            if (igUser.Pk.HasValue)
            {
                jobBuilder.UsingJobData(SchedulerKeys.InstagramUserPkKey, igUser.Pk.Value);
            }

            IJobDetail job = jobBuilder.Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(name: $"SendingStoriesTrigger_{igUser.Username}",
                              group: telegramUserId.ToString())
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(utcTime.Hour, utcTime.Minute).InTimeZone(TimeZoneInfo.Utc))
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            _logger.LogInformation("Scheduled '{JobName}' job.", jobName);
        }


        public async Task CancelStoriesSending(long telegramUserId, string instaUserId)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();

            string jobName = $"{jobPrefix}{instaUserId}";
            string triggerName = telegramUserId.ToString();

            bool IsDeleted = await scheduler.DeleteJob(new JobKey(jobName, triggerName));

            if (!IsDeleted)
                throw new InvalidOperationException("Cannot delete job " + jobName);

            _logger.LogInformation("Deleted job '{JobName}'.", jobName);
        }
    }
}
