using SimpleIptvManager.Components.Models;

namespace SimpleIptvManager.Components.Services
{
    public interface IPlaylistService
    {
        Task RemovePlaylist(int host);
        Task<PlaylistModel> AddPlaylist(string host, string username, string password);
        Task<PlaylistModel> GetPlaylistById(int id);
        Task<List<PlaylistModel>> GetAllPlaylists();
        Task ValidatePlaylist(string host, string username, string password);
        Task<List<Models.Category>> GetLiveCategoriesForPlaylistById(int id);
        Task<List<Models.Livestream>> GetLiveStreamsByCategoryId(int playlistId, int categoryId);
        Task AddCategoryToExclusions(int playlistId, int categoryId);
        Task AddCategoryFilterToExclusions(int playlistId, string filter);
        Task AddStreamToExclusions(int playlistId, int streamId);
        Task AddChannelIdOverrides(int playlistId, List<StreamChannelIdOverridesModel> overrides);
        Task CreatePlaylistAndProgramGuideForPlaylist(int playlistId);
        Task<byte[]> GetPlaylistM3uFileAsBytes(int playlistId);
        Task<byte[]> GetPlaylistEpgFileAsBytes(int playlistId);
    }
}
