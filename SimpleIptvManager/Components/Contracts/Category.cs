using System.Text.Json.Serialization;

namespace SimpleIptvManager.Components.Contracts
{
    public class Category
    {
        [JsonPropertyName("category_id")]
        public string CategoryId { get; set; }

        [JsonPropertyName("category_name")]
        public string CategoryName { get; set; }

        [JsonPropertyName("parent_id")]
        public int? ParentId { get; set; }
    }
}
