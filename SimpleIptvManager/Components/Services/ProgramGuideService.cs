using SimpleIptvManager.Components.Clients;
using System.Xml;

namespace SimpleIptvManager.Components.Services
{
    public class ProgramGuideService : IProgramGuideService
    {
        private readonly IProgramGuideClient _programGuideClient;
        public ProgramGuideService(IProgramGuideClient programGuideClient)
        {
            _programGuideClient = programGuideClient;
        }

        public async Task DownloadProgramGuide()
        {
            await _programGuideClient.DownloadUkProgramGuide();
        }

        public async void SaveProgramGuideForPlaylist(int playlistId, List<string> channelsInPlaylist)
        {
            await DownloadProgramGuide();
            var pathToFile = AppConfiguration.ProgramGuideSourcesDirectory;
            var sourceFiles = Directory.EnumerateFiles(pathToFile).ToList();
            var sourceFile = sourceFiles.First();

            var trimmedEpg = TrimEpgFile(sourceFile, channelsInPlaylist);
            CreateFile(AppConfiguration.PlaylistAndGuideDirectory, playlistId, trimmedEpg);
        }

        private void CreateFile(string path, int playlistId, XmlDocument xmlDoc)
        {
            var pathToWrite = Path.Combine(path, playlistId.ToString());
            Directory.CreateDirectory(pathToWrite);
            xmlDoc.Save(Path.Combine(pathToWrite, GetFileName(playlistId)));
        }

        public XmlDocument TrimEpgFile(string path, List<string> channelsToKeep)
        {
            var epg = new XmlDocument();
            epg.Load(path);

            RemoveNodesFromXml(epg, "tv/channel", "id", channelsToKeep);
            RemoveNodesFromXml(epg, "tv/programme", "channel", channelsToKeep);
    
            return epg;
        }

        private static void RemoveNodesFromXml(XmlDocument xml, string nodeName, string attributeName, List<string> channelsToKeep)
        {
            var nodes = xml.SelectNodes(nodeName);
            foreach (XmlNode node in nodes)
            {
                if (!channelsToKeep.Contains(node.Attributes.GetNamedItem(attributeName).Value))
                {
                    node.ParentNode.RemoveChild(node);
                }
            }
        }

        private string GetFileName(int playlistId)
        {
            return Path.Combine($"{playlistId}.xml");
        }
    }
}
