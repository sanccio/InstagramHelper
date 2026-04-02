using InstagramHelper.Core.Models;
using InstagramHelper.Core.Services.SchedulerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InstagramHelper.Core.Services.SubscriptionsService
{
    public class SubscriptionService : ISubscriptionService
    {
        readonly InstaHelperDbContext _context;
        readonly StoriesScheduler _scheduler;
        readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(InstaHelperDbContext context, StoriesScheduler scheduler, ILogger<SubscriptionService> logger)
        {
            _context = context;
            _scheduler = scheduler;
            _logger = logger;
        }


        public async Task<bool> SubscribeToInstaUserAsync(long telegramUserId, IgUserIdentifier igUser, TimeOnly time)
        {
            if (IsUserSubscribed(telegramUserId, igUser.Username))
            {
                return false;
            }

            var subscription = new Subscription()
            {
                TelegramUserId = telegramUserId,
                InstaUsername = igUser.Username,
            };

            await _context.AddAsync(subscription);
            await _context.SaveChangesAsync();

            await _scheduler.ScheduleStoriesSending(telegramUserId, igUser, time);

            _logger.LogInformation("User '{UserId}' subscribed to '@{InstaUsername}' stories.", telegramUserId, igUser.Username);

            return true;
        }


        public bool IsUserSubscribed(long telegramUserId, string instaUserId)
        {
            return _context.Subscriptions.Any(s => s.TelegramUserId == telegramUserId && s.InstaUsername == instaUserId);
        }


        public async Task<bool> Unsubscribe(long telegramUserId, string instaUserId)
        {
            if (!IsUserSubscribed(telegramUserId, instaUserId))
            {
                return false;
            }

            var subscription = _context.Subscriptions.Find(telegramUserId, instaUserId);

            if (subscription is null)
            {
                return false;
            }

            _context.Remove(subscription);
            await _context.SaveChangesAsync();

            await _scheduler.CancelStoriesSending(telegramUserId, instaUserId);

            return true;
        }


        public async Task<string[]> GetAllSubscriptionUsernamesAsync(long telegramUserId)
        {
            var usernames = await _context.Subscriptions
                .Where(s => s.TelegramUserId == telegramUserId)
                .Select(s => s.InstaUser.Username)
                .ToArrayAsync();

            return usernames;
        }
    }
}
