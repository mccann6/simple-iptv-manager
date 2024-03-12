namespace SimpleIptvManager.Components.Services
{
    public interface IProgramGuideService
    {
        Task DownloadProgramGuide();
        void SaveProgramGuideForPlaylist(int playlistId, List<string> channelsInPlaylist);
    }
}
