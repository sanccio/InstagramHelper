using InstagramHelper.Core.Models;

namespace InstagramHelper.Core.Services.TelegramServices.MediaServices;

public interface IStoryAlbumComposer
{
    Task<IEnumerable<Album>> CreateStoryAlbumsAsync(Story[] stories);
}
