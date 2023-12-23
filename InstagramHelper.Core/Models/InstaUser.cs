using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InstagramHelper.Core.Models
{
    public class InstaUser
    {
        [Key]
        [JsonPropertyName("username")]
        public string Username { get; set; } = default!;

        public ICollection<Story> Stories { get; set; } = new List<Story>();

        [NotMapped]
        [JsonPropertyName("pk")]
        public string Pk { get; set; } = default!;

        [NotMapped]
        [JsonPropertyName("biography")]
        public string? Biography { get; set; }

        [NotMapped]
        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }

        [NotMapped]
        [JsonPropertyName("is_private")]
        public bool IsPrivate { get; set; }

        [NotMapped]
        [JsonPropertyName("media_count")]
        public int MediaCount { get; set; }

        [NotMapped]
        [JsonPropertyName("follower_count")]
        public int FollowerCount { get; set; }

        [NotMapped]
        [JsonPropertyName("following_count")]
        public int FollowingCount { get; set; }

        public override string ToString()
        {
            return Pk + "\n" +
                Username + "\n" +
                Biography + "\n" +
                FullName + "\n" +
                IsPrivate + "\n" +
                MediaCount + "\n" +
                FollowerCount + "\n" +
                FollowingCount;
        }
    }

    [NotMapped]
    public class User
    {
        [JsonPropertyName("user")]
        public InstaUser? InstagramUser { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;
    }

    [NotMapped]
    public class UserResult
    {
        [JsonPropertyName("result")]
        public User? User { get; set; }
    }
}
