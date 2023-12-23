using Telegram.Bot.Types.ReplyMarkups;

namespace InstagramHelper.Core.Services.TelegramServices.Keyboards
{
    public static class UserInfoKeyboard
    {
        public static InlineKeyboardMarkup AttachInlineKeyboardMarkup =>
            new(new[]
            {
                new [] {
                    InlineKeyboardButton.WithCallbackData(text: "Get stories", callbackData: "get_stories"),
                    InlineKeyboardButton.WithCallbackData(text: "Subscribe to stories", callbackData: "subscribe"),
                },
                new [] {
                    InlineKeyboardButton.WithCallbackData(text: "Unsubscribe from stories", callbackData: "unsubscribe"),
                },
            });


        public static InlineKeyboardMarkup AttachReducedInlineKeyboardMarkup =>
            new(new[]
            {
                new [] {
                    InlineKeyboardButton.WithCallbackData(text: "Get stories", callbackData: "get_stories")
                }
            });
    }
}
