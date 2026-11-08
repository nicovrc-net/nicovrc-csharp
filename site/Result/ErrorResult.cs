using System.Text.Json.Serialization;

namespace net.nicovrc.dev.site.Result
{
    class ErrorResult
    {
        [JsonInclude]
        private string ErrorMessage = "";

        public ErrorResult(string msg)
        {
            ErrorMessage = msg;
        }

        public void Set(string msg)
        {
            ErrorMessage = msg;
        }

        public string Get()
        {
            return ErrorMessage;
        }
    }
}