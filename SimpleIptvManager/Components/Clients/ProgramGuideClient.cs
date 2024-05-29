using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace SimpleIptvManager.Components.Clients
{
    public class ProgramGuideClient : BaseClient, IProgramGuideClient
    {
        public ProgramGuideClient(HttpClient httpClient, IMemoryCache memoryCache) : base(httpClient, memoryCache) {}

        public async Task DownloadProgramGuide(int playlistId, string programGuideUrl)
        {
            var file = await DoHttpGet(programGuideUrl) ?? throw new FileNotFoundException();
            var pathToWrite = AppConfiguration.ProgramGuideSourcesDirectory;
            Directory.CreateDirectory(pathToWrite);
            var fileText = Encoding.UTF8.GetString(file);
            var cleanedFileText = fileText.Substring(0, fileText.LastIndexOf("</tv>") + 5);
            
            await File.WriteAllTextAsync(Path.Combine(pathToWrite, GetFileName(playlistId)), cleanedFileText);
        }

        private string GetFileName(int playlistId)
        {
            return $"{playlistId}_epg_{DateTime.UtcNow.ToString("yyyyMMdd")}.xml";
        }
    }
}
