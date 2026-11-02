using System;
using System.Text.RegularExpressions;

namespace net.nicovrc.dev
{
    public class HTTPFunc
    {
        private static string http_patten1 = "(GET|HEAD|OPTIONS|TRACE|PUT|DELETE|POST|PATCH|CONNECT) (.+) HTTP/(\\d\\.\\d)";

        public static byte[] CreateHTTPData(String httpVersion, int httpCode, String contentType, String contentEncoding, String redirectUrl, byte[] body)
        {

            if (httpVersion == null)
            {
                httpVersion = "1.1";
            }

            string httpMessage = "";
            if (httpCode == 200)
            {
                httpMessage = "OK";
            }
            else if (httpCode == 206)
            {
                httpMessage = "Partial Content";
            }
            else if (httpCode == 302)
            {
                httpMessage = "Found";
            }
            else if (httpCode == 400)
            {
                httpMessage = "Bad Request";
            }
            else if (httpCode == 403)
            {
                httpMessage = "Forbidden";
            }
            else if (httpCode == 404)
            {
                httpMessage = "Not Found";
            }
            else if (httpCode == 405)
            {
                httpMessage = "Method Not Allowed";
            }
            else if (httpCode == 503)
            {
                httpMessage = "Service Unavailable";
            }

            int bodyLength = body.Length;

            string httpHeader = "HTTP/" + httpVersion + " " + httpCode + " " + httpMessage + "\n";

            string httpHeader1 = "Access-Control-Allow-Origin: *";

            if (httpCode == 206)
            {
                httpHeader1 += "\nAccept-Ranges: bytes";
            }

            string date = DateTime.UtcNow.ToString("R");
            string httpHeader2 = "\nDate: " + date;

            if (contentEncoding != null)
            {
                httpHeader2 += "\nContent-Encoding: " + contentEncoding;
            }

            string httpHeader3 = "Content-Type: " + contentType + "\n" + "Content-Length: " + bodyLength;
            if (httpCode == 302 && redirectUrl != null)
            {
                httpHeader3 = "\nLocation: " + redirectUrl;
            }

            httpHeader += httpHeader1 + httpHeader2 + httpHeader3;

            byte[] header = Encoding.UTF8.GetBytes(httpHeader + "\n\n");
            byte[] result = new byte[header.Length + body.Length];

            Buffer.BlockCopy(header, 0, result, 0, header.Length);
            Buffer.BlockCopy(body, 0, result, header.Length, body.Length);

            return result;
        }

        public static string GetHTTPMethod(string httpResponse)
        {
            // GET / HTTP/1.1
            MatchCollection value = Regex.Matches(httpResponse, http_patten1);

            return value[0].Value;
        }

        public static string GetHTTPURI(string httpResponse)
        {
            // GET / HTTP/1.1
            MatchCollection value = Regex.Matches(httpResponse, http_patten1);
            return value[1].Value;
        }

        public static string GetHTTPVersion(string httpResponse)
        {
            // GET / HTTP/1.1
            MatchCollection value = Regex.Matches(httpResponse, http_patten1);
            return value[2].Value;
        }
    }
}