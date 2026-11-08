using System.Text.Json.Serialization;

namespace net.nicovrc.dev.site
{
    public class VideoResult
    {
        [JsonInclude]
        public string? VideoURL;
        
        [JsonInclude]
        public string? LiveURL;

        [JsonInclude]
        public string? AudioURL;

        [JsonInclude]
        public string[]? VideoAccessCookie;

        [JsonInclude]
        public string[]? LiveAccessCookie;

        [JsonInclude]
        public string[]? AudioAccessCookie;
    }
}