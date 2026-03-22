using InstagramHelper.Core.Models;
using InstagramHelper.Core.Services.InstagramServices.FileSizeProviders;
using InstagramHelper.Core.Services.InstagramServices.Ig;
using InstagramHelper.Core.Services.TelegramServices.Constants;
using Telegram.Bot.Types;

namespace InstagramHelper.Core.Services.TelegramServices.MediaServices;

public class StoryAlbumComposer : IStoryAlbumComposer
{
    private readonly IIgService _igService;
    private readonly IFileSizeProvider _fileSizeProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public StoryAlbumComposer(
        IIgService igService,
        IFileSizeProvider fileSizeProvider,
        IHttpClientFactory httpClientFactory)
    {
        _igService = igService;
        _fileSizeProvider = fileSizeProvider;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<Album>> CreateStoryAlbumsAsync(Story[] stories)
    {
        List<Album> albums = new();
        Album currentAlbum = new();

        long?[] fileSizes = await GetFileSizesAsync(stories);
        Stream[] videoStreams = await GetVideoStreamsAsync(stories);
        
        int videoStreamIndex = 0;

        for (int i = 0; i < stories.Length; i++)
        {
            if (ShouldSkipFile(fileSizes[i]))
            {
                continue;
            }

            long fileSize = fileSizes[i]!.Value;

            IAlbumInputMedia inputMedia = CreateInputMedia(stories[i], videoStreams, ref videoStreamIndex);

            if (!currentAlbum.TryAdd(inputMedia, fileSize))
            {
                albums.Add(currentAlbum);
                currentAlbum = new();
                currentAlbum.TryAdd(inputMedia, fileSize);
            }
        }

        if (currentAlbum.IsNotEmpty())
        {
            albums.Add(currentAlbum);
        }

        return albums;
    }


    private static bool ShouldSkipFile(long? fileSize)
    {
        return !fileSize.HasValue || fileSize > UploadLimits.MaxUploadSize;
    }


    private IAlbumInputMedia CreateInputMedia(Story story, Stream[] videoStreams, ref int videoStreamIndex)
    {
        if (!story.VideoVersions.Any())
        {
            return new InputMediaPhoto(InputFile.FromUri(story.ImageVersions.Candidates[0].Url))
            {
                Caption = _igService.GetMediaPassedTime(story.TakenAt)
            };
        }
        else
        {
            return new InputMediaVideo(InputFile.FromStream(videoStreams[videoStreamIndex++], $"{Guid.NewGuid()}.mp4"))
            {
                SupportsStreaming = true,
                Caption = _igService.GetMediaPassedTime(story.TakenAt)
            };
        }
    }


    private async Task<Stream[]> GetVideoStreamsAsync(Story[] stories)
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("StoryAlbumComposer");

        var videoStreamTasks = stories.Where(story => story.VideoVersions.Count > 0).Select(story =>
        {
            return httpClient.GetStreamAsync(story.VideoVersions[0].Url);
        });

        Stream[] videoStreams = await Task.WhenAll(videoStreamTasks);

        return videoStreams;
    }


    private async Task<long?[]> GetFileSizesAsync(Story[] stories)
    {
        var fileSizeTasks = stories.Select(story =>
        {
            if (story.VideoVersions.Count == 0)
            {
                return _fileSizeProvider.GetMediaFileSizeAsync(story.ImageVersions.Candidates[0].Url);
            }
            else
            {
                return _fileSizeProvider.GetMediaFileSizeAsync(story.VideoVersions[0].Url);
            }
        });

        long?[] fileSizes = await Task.WhenAll(fileSizeTasks);

        return fileSizes;
    }
}
