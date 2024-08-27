using InstagramHelper.Core.Models;

namespace InstagramHelper.Core.Services.TelegramServices
{
    static class BotResponse
    {
        public static string StartCommand { get; } = "Send me the username of a <u>public</u> Instagram profile to anonymously get the stories.";
        public static string BotInfoCommand { get; } = "Using this bot, you can access stories from public profiles and subscribe to recive daily updates.";
        public static string InternalServerError { get; } = "⚙️ An unexpected error has occurred.";
        public static string WaitOperationEnding { get; } = "🕵‍♂ Please wait...";
        public static string UserNotFound { get; } = "🕵‍♂ I couldn't find the user. Please check the username and try again!";
        public static string NoStoriesFound { get; } = "🕵‍♂ I couldn't find any stories!";
        public static string InvalidUsername { get; } = "⚠️ Invalid username!\nPlease send a valid Instagram username (e.g., username).\nEnsure there are no special characters or spaces in the username.";
        public static string DevelopmentStatus { get; } = "Sorry, the bot is currently under development.";
        public static string UnknownCommand { get; } = "⚠️ Unknown command.";
        public static string IncorrectTimeFormat { get; } = "⚠️ Incorrect time format. Try again!";
        public static string SubscribeSuccess { get; } = "You have subscribed to the user!";
        public static string UnsubscribeSuccess { get; } = "You have unsubscribed from the user!";
        public static string UserIsSubscribed { get; } = "✅ - you are subscribed to user stories.";
        public static string UserIsNotSubscribed { get; } = "🚫 - you are not subscribed to user stories.";

        public static string CreateShortInstaUserInfoText(string username, bool isUserSubscribed)
        {
            string subscriptionStatus = isUserSubscribed
                ? UserIsSubscribed
                : UserIsNotSubscribed;

            return $"🕵‍♂ I've found the user!\n\n" +
                $"👤 @{username}\n\n" +
                $"{subscriptionStatus}\n\n" +
                $"What should we do next?";
        }

        public static string CreateFullInstaUserInfoText(InstaUser user, bool isUserSubscribed)
        {
            string accountPrivacy = user.IsPrivate
                ? "Private account 🔒\n"
                : string.Empty;

            string fullname = !string.IsNullOrEmpty(user.FullName)
                ? $"<b>{user.FullName}</b>\n"
                : string.Empty;

            string subscriptionStatus = isUserSubscribed
                ? UserIsSubscribed
                : UserIsNotSubscribed;

            string bottomPart = user.IsPrivate
                ? "‼️ Interaction is impossible with private instagram accounts. Please try another one.\n"
                : $"{subscriptionStatus}\n\n" + "What should we do next?";

            return "🕵‍♂ I've found the user!\n\n" +
                $"<a href=\"https://www.instagram.com/{user.Username}/\">@{user.Username}</a>\n\n" +
                fullname +
                accountPrivacy +
                "------------------------------------\n" +
                $"🌅 Posts: {user.MediaCount}\n" +
                $"👥 Followers: {user.FollowerCount:n0}\n" +
                $"👀 Following: {user.FollowingCount}\n" +
                "------------------------------------\n\n" +
                $"{bottomPart}";
        }

        public static string CreateSubscriptionsListText(string[] usernames)
        {
            if (!usernames.Any())
            {
                return "📪 You don't have any subscriptions to stories yet.";
            }

            string usernamesList = "Your subscriptions:\n\n";

            foreach (var username in usernames)
                usernamesList += "👤 @" + username + "\n";

            return usernamesList;
        }

        public static string CreateTimeRequestText(string instaUsername) =>
            $"Please enter the UTC time when you'd like to receive @{instaUsername} stories. " +
            "The stories will be sent daily at the specified time. " +
            "To determine your UTC time, use the converter at https://savvytime.com/converter/utc. " +
            "Find your desired local time (e.g., 11:00), and copy corresponding UTC time directly from the converter. " +
            "Send the copied UTC time to the bot for scheduling.";
    }
}
