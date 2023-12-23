using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace InstagramHelper.Core.Services.TelegramServices.Utils
{
    public static class UpdateExtensions
    {
        public static long GetChatId(this Update update)
        {
            bool hasMessage = update.Message != null;
            bool hasCallbackMessage = update.CallbackQuery?.Message != null;
            bool hasMyChatMember = update.MyChatMember != null;

            return update.Type switch
            {
                UpdateType.Message       when hasMessage         => update.Message!.Chat.Id,
                UpdateType.CallbackQuery when hasCallbackMessage => update.CallbackQuery!.Message!.Chat.Id,
                UpdateType.MyChatMember  when hasMyChatMember    => update.MyChatMember!.Chat.Id,

                _ => throw new ArgumentOutOfRangeException(nameof(update.Type), update.Type, "Unknown update type."),
            };
        }


        public static Chat GetChat(this Update update)
        {
            bool hasMessage = update.Message != null;
            bool hasCallbackMessage = update.CallbackQuery?.Message != null;
            bool hasMyChatMember = update.MyChatMember != null;

            return update.Type switch
            {
                UpdateType.Message       when hasMessage         => update.Message!.Chat,
                UpdateType.CallbackQuery when hasCallbackMessage => update.CallbackQuery!.Message!.Chat,
                UpdateType.MyChatMember  when hasMyChatMember    => update.MyChatMember!.Chat,

                _ => throw new ArgumentOutOfRangeException(nameof(update.Type), update.Type, "Unknown update type."),
            };
        }
    }
}
