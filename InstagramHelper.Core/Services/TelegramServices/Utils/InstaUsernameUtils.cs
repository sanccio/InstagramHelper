using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;

namespace InstagramHelper.Core.Services.TelegramServices.Utils
{
    public static class InstaUsernameUtils
    {
        public static string ExtractUsernameAfterSymbol(string text, char symbol)
        {
            Match result = Regex.Match(text, $"(?<={symbol})[\\w.]+");

            return result.Value;
        }


        public static string? ValidateUsername(string instaUsername)
        {
            string invalidCharacters = "[^\\w.]";

            string username = instaUsername.StartsWith('@')
                ? instaUsername.Remove(0, 1)
                : instaUsername;

            username = username.Trim();

            if (username.IsNullOrEmpty()
                || Regex.IsMatch(username, invalidCharacters)
                || username.Length > 30)
            {
                return null;
            }

            return username.ToLower();
        }
    }
}
