using System.Text;
using System.Text.Json;
using net.nicovrc.dev.site;

namespace net.nicovrc.dev
{
    public class GetURL
    {

        public static async Task<URLResult> Run(string httpRequest, string uri)
        {

            string url = uri.Replace("/?url=", "");
            //byte[] ret = HTTPFunc.CreateHTTPData(HTTPFunc.GetHTTPVersion(httpRequest), 200, "text/plain; charset=UTF-8", null, null, Encoding.UTF8.GetBytes("url : "+url));

            //Console.WriteLine(url);
            Site site = new NicoNico();
            URLResult ret = await site.Get(url);
            VideoResult r = JsonSerializer.Deserialize<VideoResult>(Encoding.UTF8.GetString(ret.GetBody() ?? [])) ?? new VideoResult();

            
            ret.SetBody(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(r)));

            return ret;
            
        }
    }

    public class URLResult
    {
        private byte[]? body;
        private string contentType = "";

        public void SetBody(byte[] body)
        {
            this.body = body;
        }

        public byte[]? GetBody()
        {
            return body;
        }

        public void SetContentType(string contentType)
        {
            this.contentType = contentType;
        }

        public string getContentType()
        {
            return contentType;
        }

    }
}