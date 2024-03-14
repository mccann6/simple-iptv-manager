namespace SimpleIptvManager
{
    public static class AppConfiguration
    {
        public static bool IsContainer => string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")) ? false :
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER").Equals("true", StringComparison.InvariantCultureIgnoreCase) ||
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER").Equals("1", StringComparison.InvariantCultureIgnoreCase);
        public static string ApplicationName => "SimpleIptvManager";
        public static string AppDataDirectory => IsContainer ? "/config" :
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ApplicationName);
        public static string PlaylistAndGuideDirectory => Path.Combine(AppDataDirectory, "playlists");
        public static string ProgramGuideSourcesDirectory => Path.Combine(AppDataDirectory, "epgsources");
        public static string DbDirectory => Path.Combine(AppDataDirectory, "db");
        public static string DbName => "simple.db";
    }
}
