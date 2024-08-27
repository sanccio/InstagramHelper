namespace InstagramHelper.Core.Services.InstagramServices.FileSizeProviders;

public class FileSizeProvider : IFileSizeProvider
{
    private readonly HttpClient _httpClient;

    public FileSizeProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<long?> GetMediaFileSizeAsync(string mediaUrl)
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, mediaUrl);
        var response = await _httpClient.SendAsync(httpRequestMessage);

        return response.Content.Headers.ContentLength;
    }
}
