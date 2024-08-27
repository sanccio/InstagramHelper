using InstagramHelper.Core.Models;
using Telegram.Bot.Types;

namespace InstagramHelper.Core.Services.TelegramServices.MediaServices;

public interface IStoryAlbumComposer
{
    Task<IEnumerable<IEnumerable<IAlbumInputMedia>>> CreateStoryAlbumsAsync(Story[] stories);
}
