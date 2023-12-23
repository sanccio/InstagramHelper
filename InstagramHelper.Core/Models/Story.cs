using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace InstagramHelper.Core.Models
{
    public class Story
    {
        [Key]
        [JsonPropertyName("pk")]
        public string Pk { get; set; } = default!;

        [JsonPropertyName("taken_at")]
        public long TakenAt { get; set; }

        [NotMapped]
        [JsonPropertyName("image_versions2")]
        public ImageVersion ImageVersions { get; set; } = default!;

        [NotMapped]
        [JsonPropertyName("video_versions")]
        public List<VideoVersion> VideoVersions { get; set; } = new();

        [ForeignKey("InstaUser")]
        public string InstaUserId { get; set; } = default!;

        public InstaUser InstaUser { get; set; } = default!;
    }

    [NotMapped]
    public class ImageVersion
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; } = new();
    }

    [NotMapped]
    public class VideoVersion
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    [NotMapped]
    public class Candidate
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    [NotMapped]
    public class StoriesResult
    {
        [JsonPropertyName("result")]
        public List<Story> Stories { get; set; } = new();
    }
}
