using System.Text;
using System.Text.RegularExpressions;

namespace net.nicovrc.dev
{
    public class GetURL
    {

        public static byte[]? Run(string httpRequest, string uri)
        {

            string url = uri.Replace("/?url=", "");

            //Console.WriteLine(url);
            string uri_value = "url : "+url;

            byte[] ret = HTTPFunc.CreateHTTPData(HTTPFunc.GetHTTPVersion(httpRequest), 200, "text/plain; charset=UTF-8", null, null, Encoding.UTF8.GetBytes(uri_value));
            return ret;
        }
    }
}