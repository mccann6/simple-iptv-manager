using System.Text.Json.Serialization;

namespace SimpleIptvManager.Components.Contracts
{
    public class Livestream
    {
        [JsonPropertyName("num")]
        public int? Num { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("stream_type")]
        public string StreamType { get; set; }

        [JsonPropertyName("stream_id")]
        public int? StreamId { get; set; }

        [JsonPropertyName("stream_icon")]
        public string StreamIcon { get; set; }

        [JsonPropertyName("epg_channel_id")]
        public string EpgChannelId { get; set; }

        [JsonPropertyName("added")]
        public string Added { get; set; }

        [JsonPropertyName("is_adult")]
        public string IsAdult { get; set; }

        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; }

        [JsonPropertyName("custom_sid")]
        public string CustomSid { get; set; }

        [JsonPropertyName("tv_archive")]
        public int? TvArchive { get; set; }

        [JsonPropertyName("direct_source")]
        public string DirectSource { get; set; }

        //[JsonPropertyName("tv_archive_duration")]
        //public int? TvArchiveDuration { get; set; }
    }
}
