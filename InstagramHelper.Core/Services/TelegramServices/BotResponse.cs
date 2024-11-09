using InstagramHelper.Core.Enums;
using InstagramHelper.Core.Models;
using InstagramHelper.Core.Services.TelegramServices.Utils;
using System.Text;

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
        public static string UsernameRequiredMessage { get; } = "⚠️ The username must follow the command.\n\nE.g. /u <username> or /user <username>.";

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

        public static string CreateFullInstaUserInfoText(InstaUser user, AccessLevel accessLevel, bool isUserSubscribed)
        {
            string accountPrivacy = user.IsPrivate
                ? "Private account 🔒"
                : null!;

            string fullname = !string.IsNullOrEmpty(user.FullName)
                ? $"<b>{user.FullName}</b>"
                : null!;

            string subscriptionStatus = null!;

            if (accessLevel == AccessLevel.Admin || accessLevel == AccessLevel.FullAccess)
            {
                subscriptionStatus = isUserSubscribed ? UserIsSubscribed : UserIsNotSubscribed;
            }

            var builder = new StringBuilder();

            builder.AppendLine("🕵‍♂ I've found the user!")
                   .AppendLine()
                   .AppendLine($"<a href='https://www.instagram.com/{user.Username}/'>@{user.Username}</a>")
                   .AppendLine()
                   .AppendLineIfNotNullOrEmpty(fullname)
                   .AppendLineIfNotNullOrEmpty(accountPrivacy)
                   .Append("<blockquote>")
                   .AppendLine($"🌅 Posts: {user.MediaCount}")
                   .AppendLine($"👥 Followers: {user.FollowerCount:n0}")
                   .Append($"👀 Following: {user.FollowingCount}")
                   .AppendLine("</blockquote>")
                   .AppendLine();

            if (!user.IsPrivate)
            {
                builder.AppendLineIfNotNullOrEmpty(subscriptionStatus);

                if (subscriptionStatus != null)
                {
                    builder.AppendLine();
                }

                builder.Append("What should we do next?");
            }
            else
            {
                builder.Append("‼️ Interaction is impossible with private instagram accounts. Please try another one.");
            }

            return builder.ToString();
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
