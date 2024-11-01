using InstagramHelper.Core.Models;
using Microsoft.Extensions.Logging;

namespace InstagramHelper.Core.Services.InstagramServices.Ig
{
    public class IgService : IIgService
    {
        private readonly IIgApi _api;
        private readonly InstaHelperDbContext _context;
        private readonly ILogger<IgService> _logger;

        public IgService(InstaHelperDbContext context, IIgApi api, ILogger<IgService> logger)
        {
            _api = api;
            _context = context;
            _logger = logger;
        }


        public async Task<InstaUser?> GetUserAsync(string username)
        {
            try
            {
                UserResult? userResult = await _api.UserInfoByUsername(username);

                if (userResult?.User?.InstagramUser is not null)
                {
                    _logger.LogInformation("Fetched '@{Username}' info.", username);

                    return userResult.User.InstagramUser;
                }
            }
            catch (HttpRequestException httpReqEx)
            {
                _logger.LogWarning(httpReqEx, "An error occurred while fetching '@{Username}' info.", username);
            }

            return null;
        }


        public async Task<IEnumerable<Story>> GetUserStoriesAsync(string username)
        {
            try
            {
                StoriesResult? storiesResult = await _api.Stories(username);

                if (storiesResult?.Stories.Any() == true)
                {
                    _logger.LogInformation("Fetched '@{Username}' stories ({Count}).", username, storiesResult.Stories.Count);

                    return storiesResult.Stories;
                }
            }
            catch (HttpRequestException httpReqEx)
            {
                _logger.LogError(httpReqEx, "An error occurred while fetching '@{Username}' stories.", username);
            }

            return Enumerable.Empty<Story>();
        }


        public InstaUser? GetUserByUsername(string username)
        {
            return _context.InstagramUsers.SingleOrDefault(u => u.Username == username);
        }


        public async Task<bool> CreateUserIfNotExistsAsync(InstaUser instaUser)
        {
            bool userExists = _context.InstagramUsers.Any(u => u.Username == instaUser.Username);

            if (userExists)
            {
                return false;
            }

            var user = new InstaUser()
            {
                Pk = instaUser.Pk,
                Username = instaUser.Username
            };

            _context.InstagramUsers.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }


        public string GetMediaPassedTime(long takenAt)
        {
            TimeSpan passedTime = DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds(takenAt);
            var passedHours = passedTime.Hours;

            if (passedHours < 1)
            {
                return passedTime.Minutes.ToString() + " minutes ago";
            }

            return passedHours.ToString() + " hours ago";
        }


        public async Task<bool> DeleteUserAsync(string id)
        {
            InstaUser? user = await _context.InstagramUsers.FindAsync(id);

            if (user is null)
            {
                return false;
            }

            _context.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
