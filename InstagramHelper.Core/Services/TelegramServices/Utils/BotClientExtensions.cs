using Telegram.Bot.Types;
using Telegram.Bot;

namespace InstagramHelper.Core.Services.TelegramServices.Utils
{
    public static class BotClientExtensions
    {
        public static async Task SendOneOrMoreMediaGroupAsync<TMedia>(this ITelegramBotClient botClient,
                                                        long chatId,
                                                        IEnumerable<TMedia> media,
                                                        Func<TMedia[], Task<IAlbumInputMedia[]>> inputMediaCreator,
                                                        CancellationToken cancellationToken = default)
        {
            IEnumerable<TMedia[]> mediaGroups = media.Chunk(10);

            foreach (TMedia[] mediaGroup in mediaGroups)
            {
                await botClient.SendMediaGroupAsync(
                    chatId: chatId,
                    media: await inputMediaCreator(mediaGroup),
                    cancellationToken: cancellationToken);
            }
        }
    }
}
