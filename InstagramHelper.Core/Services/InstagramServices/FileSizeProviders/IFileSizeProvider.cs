namespace InstagramHelper.Core.Services.InstagramServices.FileSizeProviders;

public interface IFileSizeProvider
{
    Task<long?> GetMediaFileSizeAsync(string mediaUrl);
}
