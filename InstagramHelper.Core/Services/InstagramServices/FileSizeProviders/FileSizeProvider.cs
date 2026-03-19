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
        var headRequest = new HttpRequestMessage(HttpMethod.Head, mediaUrl);
        var response = await _httpClient.SendAsync(headRequest);

        if (response.Content.Headers.ContentLength is { } length)
            return length;

        return await DownloadAndCountBytesAsync(mediaUrl);
    }

    private async Task<long> DownloadAndCountBytesAsync(string url)
    {
        using var resp = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        resp.EnsureSuccessStatusCode();

        await using var stream = await resp.Content.ReadAsStreamAsync();

        long totalBytes = 0;
        byte[] buffer = new byte[81920];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer)) > 0)
        {
            totalBytes += bytesRead;
        }

        return totalBytes;
    }
}
