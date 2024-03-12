using Microsoft.EntityFrameworkCore;
using SimpleIptvManager.Components.Entities;
using SimpleIptvManager.Components.Models;

namespace SimpleIptvManager.Components.Data
{
    public class PlaylistDbRepository : IPlaylistRepository
    {
        private readonly SimpleIptvManagerDbContext _db = new SimpleIptvManagerDbContext();
        public async Task<PlaylistModel> CreateOrUpdate(PlaylistModel playlist)
        {
            var currentPlaylist = await _db.playlists
               .Where(p => p.PlaylistId == playlist.PlaylistId).FirstOrDefaultAsync();

            if(currentPlaylist == null)
            {
                var savedPlaylist = _db.Add(playlist.ToEntity());
                await _db.SaveChangesAsync();
                return savedPlaylist.Entity.ToModel();
            }

            var updatedPlaylistEntity = playlist.ToEntity();
            currentPlaylist.ExcludedCategoryIds = playlist.ExcludedCategoryIds;
            currentPlaylist.ExcludedStreamIds = playlist.ExcludedStreamIds;

            foreach (var o in playlist.StreamChannelIdOverrides)
            {
                var existingOverride = currentPlaylist.StreamChannelIdOverrides.Find(x => x.CategoryId == o.CategoryId && x.StreamId == o.StreamId);
                if (existingOverride != null)
                {
                    existingOverride.OriginalStreamChannelId = o.OriginalStreamChannelId;
                    existingOverride.NewStreamChannelId = o.NewStreamChannelId;
                }
                else
                {
                    currentPlaylist.StreamChannelIdOverrides.Add(new StreamChannelIdOverridesEntity
                    {
                        StreamId = o.StreamId,
                        CategoryId = o.CategoryId,
                        OriginalStreamChannelId = o.OriginalStreamChannelId,
                        NewStreamChannelId = o.NewStreamChannelId,
                    });
                }
            }

            var updatedPlaylist = _db.Update(currentPlaylist);
            await _db.SaveChangesAsync();
            return updatedPlaylist.Entity.ToModel();
        }

        public async Task Delete(int id)
        {
            var playlist = await _db.playlists
                .Where(p => p.PlaylistId == id)
                .Include(p=>p.StreamChannelIdOverrides)
                .FirstAsync();

            _db.Remove(playlist);
            await _db.SaveChangesAsync();
        }

        public Task<List<PlaylistModel>> ReadAll()
        {
            return _db.playlists.AsNoTracking().ToModel().ToListAsync();
        }

        public Task<PlaylistModel> ReadById(int id)
        {
            return _db.playlists.AsNoTracking().ToModel().SingleOrDefaultAsync(p => p.PlaylistId == id);
        }
    }
}
