using System;
using System.Text;
using System.Text.RegularExpressions;

namespace net.nicovrc.dev
{
    public class HTTPFunc
    {
        private static string http_patten1 = "(GET|HEAD|OPTIONS|TRACE|PUT|DELETE|POST|PATCH|CONNECT) (.*) HTTP/(.*)";

        public static byte[] CreateHTTPData(String? httpVersion, int httpCode, string? contentType, string? contentEncoding, string? redirectUrl, byte[] body)
        {

            httpVersion = httpVersion ?? "1.1";

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

            if (contentEncoding is not null)
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

        public static string? GetHTTPMethod(string httpResponse)
        {
            // GET / HTTP/1.1
            try
            {
                string[] h = httpResponse.Split("\n");
                Match value = Regex.Match(h[0], http_patten1, RegexOptions.IgnoreCase);
                if (value.Groups.Count == 4)
                {
                    return value.Groups[1].Value;
                }

                return null;
            } catch (Exception e)
            {
                string? stackTrace = e.StackTrace;
                Console.WriteLine(stackTrace);
                return null;
            }
        }

        public static string? GetHTTPURI(string httpResponse)
        {
            // GET / HTTP/1.1
            try
            {
                string[] h = httpResponse.Split("\n");
                //Console.WriteLine(httpResponse);
                //Console.WriteLine(h[0]);
                Match value = Regex.Match(h[0], http_patten1, RegexOptions.IgnoreCase);
                //Console.WriteLine(value.Groups.Count);
                if (value.Groups.Count == 4)
                {
                    return value.Groups[2].Value;
                }

                return null;
            } catch (Exception e)
            {
                string? stackTrace = e.StackTrace;
                Console.WriteLine(stackTrace);
                return null;
            }
        }

        public static string? GetHTTPVersion(string httpResponse)
        {
            // GET / HTTP/1.1
            try
            {
                string[] h = httpResponse.Split("\n");
                Match value = Regex.Match(h[0], http_patten1, RegexOptions.IgnoreCase);
                if (value.Groups.Count == 4)
                {
                    return value.Groups[3].Value;
                }

                return null;
            } catch (Exception e)
            {
                string? stackTrace = e.StackTrace;
                Console.WriteLine(stackTrace);
                return null;
            }
        }
    }
}