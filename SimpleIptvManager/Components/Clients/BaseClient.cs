using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace SimpleIptvManager.Components.Clients
{
    public class BaseClient
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly int _cacheExpirationMinutes = 60;

        public BaseClient(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        private Task<string> DoHttpCall(string baseUri, string resource)
        {
            return _httpClient.GetStringAsync($"{baseUri}{resource}");
        }

        protected Task<byte[]> DoHttpGet(string requestUrl)
        {
            return _httpClient.GetByteArrayAsync($"{requestUrl}");
        }

        protected async Task<T> DoCachedHttpCall<T>(string baseUri, string resource)
        {
            if (_memoryCache.TryGetValue(resource, out T response))
            {
                if(response != null)
                    return response;
            }
            var apiResponse = await DoHttpCall(baseUri, resource);

            response = JsonSerializer.Deserialize<T>(apiResponse);
            _memoryCache.Set(resource, response, TimeSpan.FromMinutes(_cacheExpirationMinutes));
            return response;
        }
    }
}
