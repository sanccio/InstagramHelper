using InstagramHelper.Core.Models;
using System.Text.Json;

namespace InstagramHelper.Core.Services.InstagramServices.Ig
{
    public class IgApi : IIgApi, IUsernameToIdResolver
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

            long userId = await UserIdByUsername(username);
            var response = await _httpClient.GetAsync($"profile?user_id={userId}");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to get user info for username '{username}' (user ID: {userId})." +
                    $" Status code: {response.StatusCode}. Response: {content}");
            }

            var instagramUser = JsonSerializer.Deserialize<InstaUser>(content, serializerOptions);

            return new UserResult
            {
                User = new User
                {
                    InstagramUser = instagramUser,
                },
            };
        }


        public async Task<StoriesResult?> Stories(IgUserIdentifier user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var response = await _httpClient.GetAsync($"stories?user_id={user.Pk}");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to get stories for user ID '{user.Pk}'." +
                    $" Status code: {response.StatusCode}. Response: {content}");
            }

            var userStories = JsonSerializer.Deserialize<List<Story>>(content, serializerOptions)
                ?? throw new JsonException($"Failed to deserialize stories for user ID '{user.Pk}'. Response: {content}");

            return new StoriesResult
            {
                Stories = userStories,
            };
        }


        public async Task<long> UserIdByUsername(string username)
        {
            ArgumentNullException.ThrowIfNull(username);

            var response = await _httpClient.GetAsync($"user_id_by_username?username={username}");

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to get user ID for username '{username}'." +
                    $" Status code: {response.StatusCode}. Response: {content}");
            }

            using var jsonDoc = JsonDocument.Parse(content);

            if (!jsonDoc.RootElement.TryGetProperty("UserID", out var userIdElement))
            {
                throw new JsonException($"User ID not found for username '{username}'. Response: {content}");
            }

            return userIdElement.GetInt64();
        }
    }
}
