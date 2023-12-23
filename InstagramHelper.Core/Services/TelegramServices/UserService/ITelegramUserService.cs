using InstagramHelper.Core.Enums;
using InstagramHelper.Core.Models;
using Telegram.Bot.Types;

namespace InstagramHelper.Core.Services.TelegramServices.UserService
{
    public interface ITelegramUserService
    {
        Task SaveUserIfNotExistsAsync(Chat chat);

        Task<TelegramUser> GetUserByIdAsync(long id);

        Task UpdateUser(TelegramUser user);

        Task<AccessLevel> GetAccessLevelAsync(long id);
    }
}
