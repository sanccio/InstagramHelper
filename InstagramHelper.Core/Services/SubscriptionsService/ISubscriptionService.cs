namespace InstagramHelper.Core.Services.SubscriptionsService
{
    public interface ISubscriptionService
    {
        Task<bool> SubscribeToInstaUserAsync(long telegramUserId, string instaUsername, TimeOnly time);

        bool IsUserSubscribed(long telegramUserId, string instaUsername);

        Task<bool> Unsubscribe(long telegramUserId, string instaUsername);

        Task<string[]> GetAllSubscriptionUsernamesAsync(long telegramUserId);
    }
}
