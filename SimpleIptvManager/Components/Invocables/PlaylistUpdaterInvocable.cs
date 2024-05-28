using Coravel.Invocable;
using SimpleIptvManager.Components.Services;

namespace SimpleIptvManager.Components.Invocables
{
    public class PlaylistUpdaterInvocable : IInvocable
    {
        private readonly IPlaylistService _playlistService;
        private readonly ILogger _logger;

        public PlaylistUpdaterInvocable(IPlaylistService playlistService, ILogger<PlaylistUpdaterInvocable> logger)
        {
            _playlistService = playlistService;
            _logger = logger;
        }

        public async Task Invoke()
        {
            var playlists = await _playlistService.GetAllPlaylists();
            foreach (var playlist in playlists)
            {
                _logger.LogInformation("Updating playlist {0} - {1}", playlist.PlaylistId, playlist.Host);
                await _playlistService.CreatePlaylistAndProgramGuideForPlaylist(playlist.PlaylistId);
                _logger.LogInformation("Playlist {0} - {1} Updated", playlist.PlaylistId, playlist.Host);
            }
        }
    }
}
