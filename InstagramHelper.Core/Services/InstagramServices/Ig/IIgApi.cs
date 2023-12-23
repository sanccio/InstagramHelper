using InstagramHelper.Core.Models;

namespace InstagramHelper.Core.Services.InstagramServices.Ig
{
    public interface IIgApi
    {
        Task<UserResult?> UserInfoByUsername(string username);

        Task<StoriesResult?> Stories(string username);
    }
}
