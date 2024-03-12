namespace SimpleIptvManager.Components.Models
{
    public class CategoryWithStreamsModel
    {
        public int Id { get; set; }
        public Category Category { get; set; }
        public List<Livestream> Livestreams { get; set; }
    }
}
