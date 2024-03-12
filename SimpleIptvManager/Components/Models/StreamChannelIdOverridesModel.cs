namespace SimpleIptvManager.Components.Models
{
    public class StreamChannelIdOverridesModel
    {
        public int StreamChannelIdOverridesId { get; set; }
        public int CategoryId { get; set; }
        public int StreamId { get; set; }
        public string? OriginalStreamChannelId { get; set; }
        public string? NewStreamChannelId { get; set; }
    }
}
