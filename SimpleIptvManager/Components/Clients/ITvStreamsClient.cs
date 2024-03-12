using SimpleIptvManager.Components.Contracts;

namespace SimpleIptvManager.Components.Clients
{
    public interface ITvStreamsClient
    {
        Task<PlaylistInfo> Authenticate(string host, string username, string password);
        Task<List<Category>> GetLiveCategories(string host, string username, string password);
        Task<List<Livestream>> GetLiveStreamsByCategory(string host, string username, string password, int categoryId);
    }
}
