using InstagramHelper.Core.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace InstagramHelper.Core.Services.TelegramServices.Keyboards
{
    public static class UserInfoKeyboard
    {
        public static InlineKeyboardMarkup AttachInlineKeyboardMarkup(IgUserIdentifier instaUser) =>
            new(new[]
            {
                new [] {
                    InlineKeyboardButton.WithCallbackData(text: "Get stories", callbackData: $"get_stories{ToCallbackSuffix(instaUser.Pk)}"),
                    InlineKeyboardButton.WithCallbackData(text: "Subscribe to stories", callbackData: $"subscribe{ToCallbackSuffix(instaUser.Pk)}"),
                },
                new [] {
                    InlineKeyboardButton.WithCallbackData(text: "Unsubscribe from stories", callbackData: "unsubscribe"),
                },
            });


        public static InlineKeyboardMarkup AttachReducedInlineKeyboardMarkup(IgUserIdentifier instaUser) =>
            new(new[]
            {
                new [] {
                    InlineKeyboardButton.WithCallbackData(text: "Get stories", callbackData: $"get_stories{ToCallbackSuffix(instaUser.Pk)}")
                }
            });


        private static string ToCallbackSuffix(long? instaUserId) =>
            instaUserId.HasValue
                ? $":{instaUserId.Value}"
                : string.Empty;
    }
}
