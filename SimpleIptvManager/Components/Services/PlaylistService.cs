using AutoMapper;
using SimpleIptvManager.Components.Clients;
using SimpleIptvManager.Components.Data;
using SimpleIptvManager.Components.Models;
using System.Globalization;
using System.Text;

namespace SimpleIptvManager.Components.Services
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IPlaylistRepository _playlistRepository;
        private readonly ITvStreamsClient _tvStreamsClient;
        private readonly IProgramGuideClient _programGuideClient;
        private readonly IMapper _mapper;
        private readonly IProgramGuideService _programGuideService;

        public PlaylistService(IPlaylistRepository playlistRepository, ITvStreamsClient client, IMapper mapper, IProgramGuideService programGuideService, IProgramGuideClient programGuideClient)
        {
            _playlistRepository = playlistRepository;
            _tvStreamsClient = client;
            _mapper = mapper;
            _programGuideService = programGuideService;
            _programGuideClient = programGuideClient;
        }
        public async Task<PlaylistModel> AddPlaylist(string host, string username, string password)
        {
            await ValidatePlaylist(host, username, password);
            var playlist = new PlaylistModel
            {
                Host = host,
                Username = username,
                Password = password
            };

            var pla = await _playlistRepository.CreateOrUpdate(playlist);

            _ = Task.Run(async () => await DownloadProgramGuideForPlaylist(pla.PlaylistId));

            return _mapper.Map<PlaylistModel>(pla);
        }

        public Task RemovePlaylist(int id)
        {
            return _playlistRepository.Delete(id);
        }

        public async Task<PlaylistModel> GetPlaylistById(int id)
        {
            return await _playlistRepository.ReadById(id);
        }

        public async Task<List<PlaylistModel>> GetAllPlaylists()
        {
            return await _playlistRepository.ReadAll();
            
        }

        public async Task ValidatePlaylist(string host, string username, string password)
        {
            try
            {
                var response = await _tvStreamsClient.Authenticate(host, username, password);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to validate playlist", ex);
            }
        }

        public async Task<List<Models.Category>> GetLiveCategoriesForPlaylistById(int id)
        {
            var playlist = await GetPlaylistById(id);
            var categoriesContract = await _tvStreamsClient.GetLiveCategories(playlist.Host, playlist.Username, playlist.Password);
            var categories = _mapper.Map<List<Models.Category>>(categoriesContract);
            return categories.Where(x => !playlist.ExcludedCategoryIds.Contains(x.CategoryId)).ToList();
        }

        public async Task<List<Models.Livestream>> GetLiveStreamsByCategoryId(int playlistId, int categoryId)
        {
            var playlist = await GetPlaylistById(playlistId);
            var livestreamsContract = await _tvStreamsClient.GetLiveStreamsByCategory(playlist.Host, playlist.Username, playlist.Password, categoryId);
            var streams = _mapper.Map<List<Models.Livestream>>(livestreamsContract);
            streams = UpdateOverridenChannelIds(playlist, streams);
            return streams.Where(x => !playlist.ExcludedStreamIds.Contains(x.StreamId)).ToList();
        }

        public List<Models.Livestream> UpdateOverridenChannelIds(Models.PlaylistModel playlist, List<Models.Livestream> streams)
        {
            foreach(var o in playlist.StreamChannelIdOverrides)
            {
                var item = streams.FirstOrDefault(x => x.CategoryId == o.CategoryId && x.StreamId == o.StreamId);
                if(item != null)
                {
                    item.EpgChannelIdOverride = o.NewStreamChannelId;
                }
            }
            return streams;
        }

        public async Task AddCategoryToExclusions(int playlistId, int categoryId)
        {
            var playlist = await GetPlaylistById(playlistId);
            playlist.ExcludedCategoryIds.Add(categoryId);

            await _playlistRepository.CreateOrUpdate(playlist);
        }

        public async Task AddCategoryFilterToExclusions(int playlistId, string filter)
        {
            var playlist = await GetPlaylistById(playlistId);
            var categories = await GetLiveCategoriesForPlaylistById(playlistId);
            var filters = filter.Split(',');
            
            var categoryIdsToKeep = categories.Where(x => filters.Any(f => x.CategoryName.Contains(f.Trim()))).Select(x=> x.CategoryId);
            var categoryIdsToExclude = categories.Where(x => !categoryIdsToKeep.Contains(x.CategoryId)).Select(x => x.CategoryId);

            playlist.ExcludedCategoryIds.AddRange(categoryIdsToExclude);

            await _playlistRepository.CreateOrUpdate(playlist);
        }

        public async Task AddStreamToExclusions(int playlistId, int streamId)
        {
            var playlist = await GetPlaylistById(playlistId);
            playlist.ExcludedStreamIds.Add(streamId);

            await _playlistRepository.CreateOrUpdate(playlist);
        }

        public async Task AddChannelIdOverrides(int playlistId, List<StreamChannelIdOverridesModel> overrides)
        {
            var playlist = await GetPlaylistById(playlistId);

            foreach (var o in overrides)
            {
                var existingOverride = playlist.StreamChannelIdOverrides.Find(x => x.CategoryId == o.CategoryId && x.StreamId == o.StreamId);
                if(existingOverride != null)
                {
                    existingOverride.OriginalStreamChannelId = o.OriginalStreamChannelId;
                    existingOverride.NewStreamChannelId = o.NewStreamChannelId;
                }
                else
                {
                    playlist.StreamChannelIdOverrides.Add(o);
                }
            }

            await _playlistRepository.CreateOrUpdate(playlist);
        }

        public async Task DownloadProgramGuideForPlaylist(int playlistId)
        {
            var playlist = await GetPlaylistById(playlistId);
            var programGuideUrl = $"{playlist.Host}/xmltv.php?username={playlist.Username}&password={playlist.Password}";

            await _programGuideClient.DownloadProgramGuide(playlistId, programGuideUrl);
        }

        private bool ShouldUpdateProgramGuideSourceForPlaylist(int playlistId)
        {
            var pathToFile = AppConfiguration.ProgramGuideSourcesDirectory;
            var sourceFiles = Directory.EnumerateFiles(pathToFile).ToList();
            var provider = CultureInfo.InvariantCulture;
            var sourceFile = sourceFiles.Where(x => Path.GetFileNameWithoutExtension(x).StartsWith($"{playlistId}_epg")).OrderByDescending(x => DateTime.ParseExact(Path.GetFileNameWithoutExtension(x).Split('_')[2], "yyyyMMdd", provider)).First();

            
            var fileName = Path.GetFileNameWithoutExtension(sourceFile);
            var dateStr = fileName.Split('_')[2];
            var dateOfFile = DateTime.ParseExact(dateStr, "yyyyMMdd", provider);

            if ((DateTime.UtcNow - dateOfFile).TotalDays >= 3)
            {
                return true;
            }

            return false;
        }


        public async Task CreatePlaylistAndProgramGuideForPlaylist(int playlistId)
        {
            var playlist = await CreatePlaylistWithCategoriesStreams(playlistId);
            var (Channels, Text) = CreateM3uText(playlist);

            CreateFile(AppConfiguration.PlaylistAndGuideDirectory, playlistId, Text);

            if (ShouldUpdateProgramGuideSourceForPlaylist(playlistId))
            {
                _ = Task.Run(async () => await DownloadProgramGuideForPlaylist(playlistId));
            }

            _programGuideService.SaveProgramGuideForPlaylist(playlistId, Channels);
        }

        private void CreateFile(string path, int playlistId, string contents)
        {
            var pathToWrite = Path.Combine(path, playlistId.ToString());
            Directory.CreateDirectory(pathToWrite);
            File.WriteAllText(Path.Combine(pathToWrite, GetFileName(playlistId)), contents);
        }

        private string GetFileName(int playlistId)
        {
            return Path.Combine($"{playlistId}.m3u");
        }

        private string GetProgramGuideFileName(int playlistId)
        {
            return Path.Combine($"{playlistId}.xml");
        }

        private async Task<PlaylistModel> CreatePlaylistWithCategoriesStreams(int playlistId)
        {
            var playlist = await GetPlaylistById(playlistId);
            var categories = await GetLiveCategoriesForPlaylistById(playlistId);
            var categoriesWithStreams = new List<CategoryWithStreamsModel>();
            foreach (var category in categories)
            {
                var livestreamsForCategory = await GetLiveStreamsByCategoryId(playlistId, category.CategoryId);
                var cc = new CategoryWithStreamsModel
                {
                    Category = category,
                    Livestreams = livestreamsForCategory
                };
                categoriesWithStreams.Add(cc);
            }
            playlist.CategoriesWithStreams = categoriesWithStreams;
            return playlist;
        }

        private (List<string> Channels, string Text) CreateM3uText(PlaylistModel playlist)
        {
            var listOfChannelsAddedToPlaylist = new List<string>();
            var sb = new StringBuilder();
            sb.AppendLine("#EXTM3U");
            foreach (var category in playlist.CategoriesWithStreams)
            {
                foreach (var livestream in category.Livestreams)
                {
                    sb.AppendLine($"#EXTINF:-1 xcms-id=\"{{XCMS_ID}}\" tvg-id=\"{livestream.EpgChannelIdOverride}\" tvg-name=\"{livestream.Name}\" tvg-logo=\"{livestream.StreamIcon}\" group-title=\"{category.Category.CategoryName}\",{livestream.Name}");
                    sb.AppendLine($"{playlist.Host}/{playlist.Username}/{playlist.Password}/{livestream.StreamId}");
                    if(livestream.EpgChannelIdOverride != null)
                        listOfChannelsAddedToPlaylist.Add(livestream.EpgChannelIdOverride);
                }
            }
            return (listOfChannelsAddedToPlaylist, sb.ToString());
        }

        public async Task<byte[]> GetPlaylistM3uFileAsBytes(int playlistId)
        {
            var pathToFile = Path.Combine(AppConfiguration.PlaylistAndGuideDirectory, playlistId.ToString());
            var maybeFilePath = Path.Combine(pathToFile, GetFileName(playlistId));
            if (File.Exists(maybeFilePath) && (DateTime.UtcNow - File.GetCreationTimeUtc(maybeFilePath) < TimeSpan.FromDays(1)))
            {
                return await File.ReadAllBytesAsync(maybeFilePath);
            }
            await CreatePlaylistAndProgramGuideForPlaylist(playlistId);
            return await File.ReadAllBytesAsync(maybeFilePath);
        }

        public async Task<byte[]> GetPlaylistEpgFileAsBytes(int playlistId)
        {
            var pathToFile = Path.Combine(AppConfiguration.PlaylistAndGuideDirectory, playlistId.ToString());
            var maybeFilePath = Path.Combine(pathToFile, GetProgramGuideFileName(playlistId));
            if (File.Exists(maybeFilePath) && (DateTime.UtcNow - File.GetCreationTimeUtc(maybeFilePath) < TimeSpan.FromDays(1)))
            {
                return await File.ReadAllBytesAsync(maybeFilePath);
            }
            await CreatePlaylistAndProgramGuideForPlaylist(playlistId);
            return await File.ReadAllBytesAsync(maybeFilePath);
        }
    }
}
