namespace SimpleIptvManager.Components.Services
{
    public interface IProgramGuideService
    {
        void SaveProgramGuideForPlaylist(int playlistId, List<string> channelsInPlaylist);
    }
}
