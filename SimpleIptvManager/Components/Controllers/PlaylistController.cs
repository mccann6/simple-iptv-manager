using SimpleIptvManager.Components.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace SimpleIptvManager.Components.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;
        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet("{id}/m3u")]
        public async Task<ActionResult> GetM3u(int id)
        {
            var file = await _playlistService.GetPlaylistM3uFileAsBytes(id);
            
            return File(file, "text/plain", $"{id}.m3u");
        }

        [HttpGet("{id}/epg")]
        public async Task<ActionResult> GetEpg(int id)
        {
            var file = await _playlistService.GetPlaylistEpgFileAsBytes(id);
            return Content(Encoding.UTF8.GetString(file), "text/xml");
        }
    }
}
