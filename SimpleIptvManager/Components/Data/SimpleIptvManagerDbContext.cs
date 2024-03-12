using Microsoft.EntityFrameworkCore;
using SimpleIptvManager.Components.Entities;

namespace SimpleIptvManager.Components.Data
{
    public class SimpleIptvManagerDbContext : DbContext
    {
        public DbSet<PlaylistEntity> playlists { get; set; }
        private string DbPath { get; }

        public SimpleIptvManagerDbContext()
        {
            var path = Path.Combine(AppConfiguration.AppDataDirectory, AppConfiguration.DbDirectory);
            Directory.CreateDirectory(path);
            DbPath = Path.Join(path, AppConfiguration.DbName);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");
    }
}
