using InstagramHelper.Core.Models;
using InstagramHelper.Core.Services.SchedulerService;
using Microsoft.EntityFrameworkCore;

namespace InstagramHelper.Core.Services.SubscriptionsService
{
    public class SubscriptionService : ISubscriptionService
    {
        readonly InstaHelperDbContext _context;
        readonly StoriesScheduler _scheduler;

        public SubscriptionService(InstaHelperDbContext context, StoriesScheduler scheduler)
        {
            _context = context;
            _scheduler = scheduler;
        }


        public async Task<bool> SubscribeToInstaUserAsync(long telegramUserId, string instaUserId, TimeOnly time)
        {
            if (IsUserSubscribed(telegramUserId, instaUserId))
            {
                return false;
            }

            var subscription = new Subscription()
            {
                TelegramUserId = telegramUserId,
                InstaUsername = instaUserId
            };

            await _context.AddAsync(subscription);
            await _context.SaveChangesAsync();

            await _scheduler.ScheduleStoriesSending(telegramUserId, instaUserId, time);

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
