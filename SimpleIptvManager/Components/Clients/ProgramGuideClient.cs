﻿using Microsoft.Extensions.Caching.Memory;
using System.IO.Compression;

namespace SimpleIptvManager.Components.Clients
{
    public class ProgramGuideClient : BaseClient, IProgramGuideClient
    {
        private readonly string _programGuideSource = "https://epgshare01.online/epgshare01/";
        private readonly string _ieProgramGuide = "IE1";
        private readonly string _ukProgramGuide = "UK1";
        private readonly string _usProgramGuide = "US1";

        public ProgramGuideClient(HttpClient httpClient, IMemoryCache memoryCache) : base(httpClient, memoryCache) {}

        public async Task DownloadUkProgramGuide()
        {
            await DownloadProgramGuide(_ukProgramGuide);
        }

        private async Task DownloadProgramGuide(string programGuideName)
        {
            var file = await DoHttpGet($"{_programGuideSource}epg_ripper_{programGuideName}.xml.gz")
                ?? throw new FileNotFoundException();
            var decompressedFile = DecompressFile(file);
            await CreateFile(AppConfiguration.ProgramGuideSourcesDirectory, decompressedFile);
        }

        private async Task CreateFile(string path, byte[] contents)
        {
            var pathToWrite = Path.Combine(path);
            Directory.CreateDirectory(pathToWrite);
            await File.WriteAllBytesAsync(Path.Combine(pathToWrite, GetFileName()), contents);
        }

        private string GetFileName()
        {
            return $"uk_epg_{DateTime.UtcNow.ToString("yyyyMMdd")}.xml";
        }

        private byte[] DecompressFile(byte[] compressedFileBytes)
        {
            using (var compressedStream = new MemoryStream(compressedFileBytes))
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                zipStream.CopyTo(resultStream);
                return resultStream.ToArray();
            }
        }
    }
}
