namespace InstagramHelper.Core.Services.InstagramServices.Ig;

public interface IUsernameToIdResolver
{
    Task<long> UserIdByUsername(string username);
}
