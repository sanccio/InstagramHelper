using Telegram.Bot.Types;
using Telegram.Bot;

namespace InstagramHelper.Core.Services.TelegramServices.Utils
{
    public static class BotClientExtensions
    {
        public static async Task SendOneOrMoreMediaGroupAsync<TMedia>(this ITelegramBotClient botClient,
                                                        long chatId,
                                                        TMedia[] media,
                                                        Func<TMedia[], Task<IEnumerable<IEnumerable<IAlbumInputMedia>>>> inputMediaCreator,
                                                        CancellationToken cancellationToken = default)
        {
            IEnumerable<IEnumerable<IAlbumInputMedia>> mediaGroups = await inputMediaCreator(media);

            foreach (IEnumerable<IAlbumInputMedia> mediaGroup in mediaGroups)
            {
                await botClient.SendMediaGroupAsync(
                    chatId: chatId,
                    media: mediaGroup,
                    cancellationToken: cancellationToken);
            }
        }
    }
}
