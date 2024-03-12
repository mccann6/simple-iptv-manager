using System.Text.Json.Serialization;

namespace SimpleIptvManager.Components.Contracts
{
    public class PlaylistInfo
    {
        [JsonPropertyName("user_info")]
        public UserInfo UserInfo { get; set; }

        [JsonPropertyName("server_info")]
        public Info ServerInfo { get; set; }
    }

    public class Info
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("port")]
        public string Port { get; set; }

        [JsonPropertyName("https_port")]
        public string HttpsPort { get; set; }

        [JsonPropertyName("server_protocol")]
        public string ServerProtocol { get; set; }

        [JsonPropertyName("rtmp_port")]
        public string RtmpPort { get; set; }

        [JsonPropertyName("timezone")]
        public string Timezone { get; set; }

        [JsonPropertyName("timestamp_now")]
        public int? TimestampNow { get; set; }

        [JsonPropertyName("time_now")]
        public string TimeNow { get; set; }

        [JsonPropertyName("process")]
        public bool Process { get; set; }
    }

    public class UserInfo
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("auth")]
        public int? Auth { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("exp_date")]
        public string ExpDate { get; set; }

        [JsonPropertyName("is_trial")]
        public string IsTrial { get; set; }

        [JsonPropertyName("active_cons")]
        public string ActiveCons { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("max_connections")]
        public string MaxConnections { get; set; }

        [JsonPropertyName("allowed_output_formats")]
        public List<string> AllowedOutputFormats { get; set; }
    }


}
