using InstagramHelper.Core.Enums;
using InstagramHelper.Core.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace InstagramHelper.Core.Services.TelegramServices.UserService
{
    public class TelegramUserService : ITelegramUserService
    {
        readonly InstaHelperDbContext _context;

        public TelegramUserService(InstaHelperDbContext context)
        {
            _context = context;
        }


        public async Task SaveUserIfNotExistsAsync(Chat chat)
        {
            bool userExists = await _context.TelegramUsers.AnyAsync(tu => tu.Id == chat.Id);

            if (userExists)
            {
                return;
            }

            TelegramUser telegramUser = new()
            {
                Id = chat.Id,
                FirstName = chat.FirstName,
                LastName = chat.LastName,
                Username = chat.Username,
                AccessLevel = AccessLevel.LimitedAccess
            };

            await _context.TelegramUsers.AddAsync(telegramUser);
            await _context.SaveChangesAsync();
        }


        public async Task<TelegramUser> GetUserByIdAsync(long id)
        {
            return await _context.TelegramUsers.FindAsync(id)
                ?? throw new Exception($"The telegram user with id '{id}' was not found.");
        }


        public async Task UpdateUser(TelegramUser user)
        {
            _context.TelegramUsers.Update(user);
            await _context.SaveChangesAsync();
        }


        public async Task<AccessLevel> GetAccessLevelAsync(long id)
        {
            TelegramUser? user = await _context.TelegramUsers.FindAsync(id);

            if (user is null) 
            { 
                return AccessLevel.LimitedAccess; 
            }

            return user.AccessLevel;
        }
    }
}
