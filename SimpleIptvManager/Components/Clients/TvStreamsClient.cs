using SimpleIptvManager.Components.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace SimpleIptvManager.Components.Clients
{
    public class TvStreamsClient : BaseClient, ITvStreamsClient
    {
        private readonly string _api = "player_api.php";
        private readonly string _getLiveCategoriesAction = "get_live_categories";
        private readonly string _getLiveStreams = "get_live_streams";

        public TvStreamsClient(HttpClient httpClient, IMemoryCache memoryCache, ILogger<BaseClient> logger) : base(httpClient, memoryCache, logger){}

        public Task<PlaylistInfo> Authenticate(string host, string username, string password)
        {
            return DoCachedHttpCall<PlaylistInfo>(host, $"/{_api}?username={username}&password={password}");
        }

        public Task<List<Category>> GetLiveCategories(string host, string username, string password)
        {
            return DoCachedHttpCall<List<Category>>(host, $"/{_api}?username={username}&password={password}&action={_getLiveCategoriesAction}");
        }

        public Task<List<Livestream>> GetLiveStreamsByCategory(string host, string username, string password, int categoryId)
        {
            return DoCachedHttpCall<List<Livestream>>(host, $"/{_api}?username={username}&password={password}&action={_getLiveStreams}&category_id={categoryId}");
        }
    }
}
