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

    public StoryAlbumComposer(IIgService igService, IFileSizeProvider fileSizeProvider, IHttpClientFactory httpClientFactory)
    {
        _igService = igService;
        _fileSizeProvider = fileSizeProvider;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IEnumerable<IEnumerable<IAlbumInputMedia>>> CreateStoryAlbumsAsync(Story[] stories)
    {
        var albums = new List<IEnumerable<IAlbumInputMedia>>();
        var currentAlbum = new List<IAlbumInputMedia>();
        long albumSummarySize = 0;

        HttpClient httpClient = _httpClientFactory.CreateClient("StoryAlbumComposer");

        long?[] fileSizes = await GetFileSizesAsync(stories);
        Stream[] videoStreams = await GetVideoStreamsAsync(stories, httpClient);

        int videoStreamIndex = 0;

        for (int i = 0; i < stories.Length; i++)
        {
            long? fileSize = fileSizes[i];

            if (!fileSize.HasValue || fileSize > UploadLimits.MaxUploadSize)
            {
                continue;
            }

            IAlbumInputMedia inputMedia = CreateAlbumInputMediaItem(stories[i], videoStreams, ref videoStreamIndex);

            if (albumSummarySize + fileSize.Value <= UploadLimits.MaxUploadSize && currentAlbum.Count < UploadLimits.MaxAlbumMediaCount)
            {
                albumSummarySize += fileSize.Value;
                currentAlbum.Add(inputMedia);
            }
            else
            {
                albums.Add(currentAlbum);
                currentAlbum = new List<IAlbumInputMedia> { inputMedia };
                albumSummarySize = fileSize.Value;
            }
        }

        if (currentAlbum.Any())
        {
            albums.Add(currentAlbum);
        }

        return albums;
    }


    private IAlbumInputMedia CreateAlbumInputMediaItem(Story story, Stream[] videoStreams, ref int videoStreamIndex)
    {
        if (!story.VideoVersions.Any())
        {
            return new InputMediaPhoto(InputFile.FromUri(story.ImageVersions.Candidates[1].Url))
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


    private static async Task<Stream[]> GetVideoStreamsAsync(Story[] stories, HttpClient httpClient)
    {
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
                return _fileSizeProvider.GetMediaFileSizeAsync(story.ImageVersions.Candidates[1].Url);
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
