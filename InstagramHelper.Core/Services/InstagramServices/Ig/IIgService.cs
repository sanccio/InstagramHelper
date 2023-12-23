using InstagramHelper.Core.Models;

namespace InstagramHelper.Core.Services.InstagramServices.Ig
{
    public interface IIgService
    {
        Task<InstaUser?> GetUserAsync(string username);

        Task<IEnumerable<Story>> GetUserStoriesAsync(string username);

        string GetMediaPassedTime(long takenAt);

        Task<bool> CreateUserIfNotExistsAsync(InstaUser instaUser);

        Task<bool> DeleteUserAsync(string id);
    }
}
