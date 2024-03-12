using SimpleIptvManager.Components.Data;

namespace SimpleIptvManager.Components.Models
{
    public class Livestream
    {
        public int LivestreamId { get; set; }
        public int? Num { get; set; }
        public string Name { get; set; }
        public string StreamType { get; set; }
        public int StreamId { get; set; }
        public string StreamIcon { get; set; }
        public string EpgChannelId { get; set; }

        private string _epgChannelIdOverride;
        public string EpgChannelIdOverride
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_epgChannelIdOverride))
                {
                    if (ChannelIds.UkChannelIds.Contains(EpgChannelId))
                    {
                        return EpgChannelId;
                    }

                    return _epgChannelIdOverride;
                }
                else
                {
                    return _epgChannelIdOverride;
                }
            }
            set
            {
                this._epgChannelIdOverride = value;
            }
        }

        public string Added { get; set; }
        public bool IsAdult { get; set; }
        public int? CategoryId { get; set; }
        public string CustomSid { get; set; }
        public int? TvArchive { get; set; }
        public string DirectSource { get; set; }
        //public int? TvArchiveDuration { get; set; }
    }
}
