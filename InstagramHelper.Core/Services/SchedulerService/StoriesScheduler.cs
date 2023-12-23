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


        public async Task ScheduleStoriesSending(long telegramUserId, string instaUserId, TimeOnly utcTime)
        {
            IScheduler scheduler = await _schedulerFactory.GetScheduler();

            string jobName = $"{jobPrefix}{instaUserId}";

            IJobDetail job = JobBuilder.Create<StoriesSender>()
                    .WithIdentity(name: jobName,
                                  group: telegramUserId.ToString())
                    .UsingJobData(SchedulerKeys.InstagramUserIdKey, instaUserId)
                    .UsingJobData(SchedulerKeys.TelegramUserIdKey, telegramUserId)
                    .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(name: $"SendingStoriesTrigger_{instaUserId}",
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
