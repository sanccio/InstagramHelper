using InstagramHelper.Core.Models;

namespace InstagramHelper.Core.Services.SubscriptionsService
{
    public interface ISubscriptionService
    {
        Task<bool> SubscribeToInstaUserAsync(long telegramUserId, IgUserIdentifier igUser, TimeOnly time);

        bool IsUserSubscribed(long telegramUserId, string instaUsername);

        Task<bool> Unsubscribe(long telegramUserId, string instaUsername);

        Task<string[]> GetAllSubscriptionUsernamesAsync(long telegramUserId);
    }
}
