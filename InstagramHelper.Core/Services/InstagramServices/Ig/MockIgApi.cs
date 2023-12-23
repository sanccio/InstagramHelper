using InstagramHelper.Core.Models;
using System.Text.Json;

namespace InstagramHelper.Core.Services.InstagramServices.Ig
{
    public class MockIgApi : IIgApi
    {
        public Task<UserResult?> UserInfoByUsername(string username)
        {
            ArgumentNullException.ThrowIfNull(username);

            return Task.FromResult<UserResult?>(new UserResult
            {
                User = new User
                {
                    InstagramUser = new InstaUser
                    {
                        Pk = "1234567",
                        Username = "tonystark",
                        Biography = "Engineer",
                        FullName = "Tony Stark",
                        IsPrivate = false,
                        MediaCount = 100,
                        FollowerCount = 5000000,
                        FollowingCount = 200
                    },
                    Status = "Ok"
                }
            });
        }


        public Task<StoriesResult?> Stories(string username)
        {
            ArgumentNullException.ThrowIfNull(username);

            return Task.FromResult<StoriesResult?>(new StoriesResult
            {
                Stories = new List<Story>
                {
                    new Story
                    {
                        Pk = "3243224315850640841",
                        TakenAt = 0,
                        ImageVersions = new ImageVersion
                        {
                            Candidates = new List<Candidate>
                            {
                                new Candidate
                                {
                                    Url = "https://media.sproutsocial.com/uploads/2022/12/IMG_6187.png",
                                    Width = 677,
                                    Height = 1466
                                },
                                new Candidate
                                {
                                    Url = "https://media.sproutsocial.com/uploads/2022/12/IMG_6187.png",
                                    Width = 677,
                                    Height = 1466
                                }
                            }
                        },
                        InstaUser = new InstaUser
                        {
                            Pk = "1234567",
                            Username = "tonystark"
                        }
                    }
                }
            });
        }


        public static async Task<T> DeserializeFile<T>(string path)
        {
            using FileStream openStream = File.OpenRead(path);

            var result = await JsonSerializer.DeserializeAsync<T>(openStream);

            return result!;
        }
    }
}
