using InstagramHelper.Core.Models;
using System.Text.Json;

namespace InstagramHelper.Core.Services.InstagramServices.Ig
{
    public class IgApi : IIgApi
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions serializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public IgApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<UserResult?> UserInfoByUsername(string username)
        {
            ArgumentNullException.ThrowIfNull(username);

            var path = $"userInfoByUsername/{username}";
            var response = await _httpClient.GetAsync(path);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{response.Content}");
            }

            var stringResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<UserResult>(stringResponse, serializerOptions);
        }


        public async Task<StoriesResult?> Stories(string username)
        {
            ArgumentNullException.ThrowIfNull(username);

            var path = $"story?url=https://www.instagram.com/stories/{username}/";
            var response = await _httpClient.GetAsync(path);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{response.StatusCode} {response.Content}");
            }

            var stringResponse = await response.Content.ReadAsStringAsync();

            var storiesResult = JsonSerializer.Deserialize<StoriesResult>(stringResponse, serializerOptions);

            return storiesResult;
        }
    }
}
