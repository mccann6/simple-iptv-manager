namespace SimpleIptvManager.Components.Clients
{
    public interface IProgramGuideClient
    {
        Task DownloadProgramGuide(int playlistId, string programGuideUrl);
        Task DownloadUkProgramGuide();
    }
}
