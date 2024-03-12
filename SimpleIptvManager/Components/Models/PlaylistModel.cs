using SimpleIptvManager.Components.Entities;

namespace SimpleIptvManager.Components.Models
{
    public class PlaylistModel
    {
        public int PlaylistId { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<int> ExcludedCategoryIds { get; set; } = new List<int>();
        public List<int> ExcludedStreamIds { get; set; } = new List<int>();
        public List<StreamChannelIdOverridesModel> StreamChannelIdOverrides { get; set; } = new List<StreamChannelIdOverridesModel>();
        public List<CategoryWithStreamsModel> CategoriesWithStreams { get; set; } = new List<CategoryWithStreamsModel>();
    }

    public static class PlaylistModelExtentions
    {
        public static IQueryable<PlaylistModel> ToModel(this IQueryable<PlaylistEntity> entity)
        {
            return entity.Select(e => new PlaylistModel
            {
                PlaylistId = e.PlaylistId,
                Host = e.Host,
                Username = e.Username,
                Password = e.Password,
                ExcludedCategoryIds = e.ExcludedCategoryIds,
                ExcludedStreamIds = e.ExcludedStreamIds,
                StreamChannelIdOverrides = e.StreamChannelIdOverrides.Select(s => new StreamChannelIdOverridesModel
                {
                    StreamChannelIdOverridesId = s.StreamChannelIdOverridesId,
                    CategoryId = s.CategoryId,
                    StreamId = s.StreamId,
                    OriginalStreamChannelId = s.OriginalStreamChannelId,
                    NewStreamChannelId = s.NewStreamChannelId
                }).ToList()
            });
        }

        public static PlaylistModel ToModel(this PlaylistEntity entity)
        {
            return new PlaylistModel
            {
                PlaylistId = entity.PlaylistId,
                Host = entity.Host,
                Username = entity.Username,
                Password = entity.Password,
                ExcludedCategoryIds = entity.ExcludedCategoryIds,
                ExcludedStreamIds = entity.ExcludedStreamIds,
                StreamChannelIdOverrides = entity.StreamChannelIdOverrides.Select(s => new StreamChannelIdOverridesModel
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
