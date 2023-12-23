using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using Telegram.Bot;

namespace InstagramHelper.Core.Services.TelegramServices.Utils
{
    public class InstaUsernameHelper
    {
        public static string ExtractUsernameAfterSymbol(string text, char symbol)
        {
            Match result = Regex.Match(text, $"(?<={symbol})[\\w.]+");

            return result.Value;
        }


        public static async Task<string?> ValidateUsername(ITelegramBotClient botClient,
                                                           string instaUsername,
                                                           long chatId)
        {
            string invalidCharacters = "[^\\w.]";

            string username = instaUsername.StartsWith('@')
                ? instaUsername.Remove(0, 1)
                : instaUsername;

            if (username.IsNullOrEmpty() || Regex.IsMatch(username, invalidCharacters))
            {
                await botClient.SendTextMessageAsync(chatId, BotResponse.InvalidUsername);

                return null;
            }

            return username.ToLower();
        }
    }
}
