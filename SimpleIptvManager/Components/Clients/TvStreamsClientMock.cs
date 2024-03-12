using SimpleIptvManager.Components.Contracts;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Text.Json;

namespace SimpleIptvManager.Components.Clients
{
    public class TvStreamsClientMock : BaseClient, ITvStreamsClient
    {
        private readonly string CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private const string SampleFilesLocation = "Components\\Clients\\SampleResponses\\";
        private const string AuthenticateSampleFileName = "authenticate.json";
        private const string LiveCategoriesSampleFileName = "get_live_categories.json";
        private const string LiveStreamsByCategorySampleFileName = "get_live_streams_category.json";
        
        public TvStreamsClientMock(HttpClient httpClient, IMemoryCache memoryCache, ILogger<BaseClient> logger) : base(httpClient, memoryCache, logger)
        {
        }

        public Task<PlaylistInfo> Authenticate(string host, string username, string password) 
            => Task.FromResult(GetResponseFromFile<PlaylistInfo>(AuthenticateSampleFileName));

        public Task<List<Category>> GetLiveCategories(string host, string username, string password) 
            => Task.FromResult(GetResponseFromFile<List<Category>>(LiveCategoriesSampleFileName));

        public Task<List<Livestream>> GetLiveStreamsByCategory(string host, string username, string password, int categoryId)
        {
            var response = GetResponseFromFile<List<Livestream>>(LiveStreamsByCategorySampleFileName);
            var test = response.Where(x => int.Parse(x.CategoryId) == categoryId).ToList();

            return Task.FromResult(response.Where(x => int.Parse(x.CategoryId) == categoryId).ToList());
        }

        private T GetResponseFromFile<T>(string fileName)
        {
            string path = Path.Combine(CurrentDirectory, SampleFilesLocation, fileName);
            string responseStr = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(responseStr);
        }
    }
}
