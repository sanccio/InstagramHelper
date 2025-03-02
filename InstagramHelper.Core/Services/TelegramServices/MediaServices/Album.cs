using Telegram.Bot.Types;
using InstagramHelper.Core.Services.TelegramServices.Constants;
using System.Collections;

namespace InstagramHelper.Core.Services.TelegramServices.MediaServices;

public class Album : IEnumerable<IAlbumInputMedia>
{
    private readonly List<IAlbumInputMedia> _album = new();

    private long _albumSize = 0;

    public bool TryAdd(IAlbumInputMedia media, long mediaSize)
    {
        if (CanAddMedia(mediaSize))
        {
            _album.Add(media);
            _albumSize += mediaSize;

            return true;
        }

        return false;
    }

    public bool IsNotEmpty() => _album.Any();

    public IEnumerator<IAlbumInputMedia> GetEnumerator() => _album.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private bool CanAddMedia(long mediaSize)
    {
        return _albumSize + mediaSize <= UploadLimits.MaxUploadSize 
            && _album.Count < UploadLimits.MaxAlbumMediaCount;
    }
}
