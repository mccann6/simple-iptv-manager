using SimpleIptvManager.Components.Models;

namespace SimpleIptvManager.Components.Data
{
    public interface IPlaylistRepository
    {
        Task<PlaylistModel> CreateOrUpdate(PlaylistModel playlist);
        Task<PlaylistModel> ReadById(int id);
        Task<List<PlaylistModel>> ReadAll();
        Task Delete(int id);
    }
}
