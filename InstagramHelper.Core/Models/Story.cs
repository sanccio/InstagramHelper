using System.Text.Json.Serialization;

namespace InstagramHelper.Core.Models
{
    public class Story
    {
        [JsonPropertyName("pk")]
        public long Pk { get; set; } = default!;

        [JsonPropertyName("taken_at")]
        public long TakenAt { get; set; }

        [JsonPropertyName("image_versions2")]
        public ImageVersion ImageVersions { get; set; } = default!;

        [JsonPropertyName("video_versions")]
        public List<VideoVersion> VideoVersions { get; set; } = new();
    }

    public class ImageVersion
    {
        [JsonPropertyName("candidates")]
        public List<Candidate> Candidates { get; set; } = new();
    }

    public class VideoVersion
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("url")]
        public string Url { get; set; } = default!;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class StoriesResult
    {
        [JsonPropertyName("result")]
        public List<Story> Stories { get; set; } = new();
    }
}
