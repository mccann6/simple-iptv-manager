using SimpleIptvManager.Components.Models;
using System.ComponentModel.DataAnnotations;

namespace SimpleIptvManager.Components.Entities
{
    public class PlaylistEntity
    {
        [Key]
        public int PlaylistId { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<int> ExcludedCategoryIds { get; set; } = new List<int>();
        public List<int> ExcludedStreamIds { get; set; } = new List<int>();
        public List<StreamChannelIdOverridesEntity> StreamChannelIdOverrides { get; set; } = new List<StreamChannelIdOverridesEntity>();
    }

    public static class PlaylistEntityExtentions
    {
        public static PlaylistEntity ToEntity(this PlaylistModel model)
        {
            return new PlaylistEntity
            {
                PlaylistId = model.PlaylistId,
                Host = model.Host,
                Username = model.Username,
                Password = model.Password,
                ExcludedCategoryIds = model.ExcludedCategoryIds,
                ExcludedStreamIds = model.ExcludedStreamIds,
                StreamChannelIdOverrides = model.StreamChannelIdOverrides.Select(s => new StreamChannelIdOverridesEntity
                {
                    StreamChannelIdOverridesId = s.StreamChannelIdOverridesId,
                    CategoryId = s.CategoryId,
                    StreamId = s.StreamId,
                    OriginalStreamChannelId = s.OriginalStreamChannelId,
                    NewStreamChannelId = s.NewStreamChannelId
                }).ToList()
            };
        }
    }
}
