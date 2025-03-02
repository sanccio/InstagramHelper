using Telegram.Bot;
using InstagramHelper.Core.Services.TelegramServices.MediaServices;

namespace InstagramHelper.Core.Services.TelegramServices.Utils
{
    public static class BotClientExtensions
    {
        public static async Task SendOneOrMoreMediaGroupAsync<TMedia>(this ITelegramBotClient botClient,
                                                        long chatId,
                                                        TMedia[] media,
                                                        Func<TMedia[], Task<IEnumerable<Album>>> inputMediaCreator,
                                                        CancellationToken cancellationToken = default)
        {
            IEnumerable<Album> mediaGroups = await inputMediaCreator(media);

            foreach (Album mediaGroup in mediaGroups)
            {
                await botClient.SendMediaGroupAsync(
                    chatId: chatId,
                    media: mediaGroup,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
