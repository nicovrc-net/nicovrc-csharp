using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using net.nicovrc.dev.site.Result;

namespace net.nicovrc.dev.site
{
    public class NicoNico : Site
    {
        public async Task<URLResult> Get(string url)
        {
            URLResult result = new URLResult();
            result.SetContentType("application/json; charset=utf-8");

            //Console.WriteLine("debug-0");
            string json = await Run(url);

            //Console.WriteLine("debug-1");
            try
            {
                //Console.WriteLine(json);
                // ErrorMessage
                Match match = Regex.Match(json, "(E|e)rror(M|m)essage");
                if (!match.Success)
                {
                    //Console.WriteLine("!");
                    result.SetBody(Encoding.UTF8.GetBytes(json));
                }
                else
                {
                    ErrorResult error = JsonSerializer.Deserialize<ErrorResult>(json) ?? new ErrorResult("");
                    result.SetBody(Encoding.UTF8.GetBytes(error.Get()));
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.StackTrace);
                result.SetBody(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new ErrorResult(e.StackTrace ?? "内部エラー"))));
            }

            return result;
        }

        public async Task<string> GetJson(string url)
        {

            return await Run(url);
        }

        private async Task<string> Run(string url)
        {
            //Console.WriteLine("debug");
            bool Found = false;
            if (url.StartsWith("http://nico.ms") || url.StartsWith("http://www.nicovideo.jp") || url.StartsWith("http://live.nicovideo.jp") || url.StartsWith("https://nico.ms") || url.StartsWith("https://www.nicovideo.jp") || url.StartsWith("https://live.nicovideo.jp"))
            {
                Found = true;
            }

            if (!Found)
            {
                return JsonSerializer.Serialize(new ErrorResult("対応していないURLです。"));
            }

            //Console.WriteLine(url);

            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd(NicovrcSetting.UserAgent);
                client.Timeout = TimeSpan.FromSeconds(5);

                if (NicovrcSetting.NicoNico__nicosid is not null && NicovrcSetting.NicoNico__user_session is not null)
                {
                    client.DefaultRequestHeaders.Add("Cookie", "nicosid=" + NicovrcSetting.NicoNico__nicosid + "; user_session=" + NicovrcSetting.NicoNico__user_session);
                }
                NicoNicoResult result = new NicoNicoResult();

                HttpResponseMessage httpResponseMessage = await client.GetAsync(url);

                if (httpResponseMessage.RequestMessage is not null && httpResponseMessage.RequestMessage.RequestUri is not null && httpResponseMessage.RequestMessage.RequestUri.Host == "www.nicovideo.jp")
                {
                    // ニコ動
                    string response = "";
                    if (((int)httpResponseMessage.StatusCode) >= 200 && ((int)httpResponseMessage.StatusCode) < 300)
                    {
                        response = await httpResponseMessage.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return JsonSerializer.Serialize(new ErrorResult("HTTPエラー : " + httpResponseMessage.StatusCode));
                    }

                    string match1 = @"<meta name=""server-response"" content=""\{(.+?)\}""\s*/?>";
                    Match match = Regex.Match(response, match1);

                    string jsonText = "";
                    if (match.Groups.Count > 0)
                    {
                        //Console.WriteLine(match.Groups[1].Value);
                        jsonText = "{" + match.Groups[1].Value.Replace("&quot;", "\"") + "}";
                    }

                    //Console.WriteLine(jsonText);
                    JsonDocument json = JsonDocument.Parse(jsonText);
                    string nicosid = json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("client").GetProperty("nicosid").GetString() ?? "";
                    string videoId = json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("video").GetProperty("id").GetString() ?? "";
                    string watchTrackId = json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("client").GetProperty("watchTrackId").GetString() ?? "";

                    result.SetURL("https://nico.ms/" + videoId);
                    result.SetTitle(json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("video").GetProperty("title").GetString() ?? "");
                    result.SetDescription(json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("video").GetProperty("description").GetString() ?? "");

                    foreach (JsonElement element in json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("tags").GetProperty("items").EnumerateArray())
                    {
                        result.AddTag(element.GetProperty("name").GetString() ?? "");
                    }

                    result.SetThumbnail(json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("video").GetProperty("thumbnail").GetProperty("player").GetString() ?? "");

                    result.SetViewCount(json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("video").GetProperty("count").GetProperty("view").GetInt64());
                    result.SetCommentCount(json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("video").GetProperty("count").GetProperty("comment").GetInt64());
                    result.SetMyListCount(json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("video").GetProperty("count").GetProperty("mylist").GetInt64());
                    result.SetLikeCount(json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("video").GetProperty("count").GetProperty("like").GetInt64());

                    result.SetLikeCount(json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("video").GetProperty("duration").GetInt64());

                    string videoHlsData = "{\"outputs\":[";
                    //Console.Write(videoHlsData);

                    foreach (JsonElement video in json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("media").GetProperty("contents").GetProperty("videos").EnumerateArray())
                    {
                        if (video.GetProperty("isAvailable").GetBoolean())
                        {
                            foreach (JsonElement audio in json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("media").GetProperty("contents").GetProperty("audios").EnumerateArray())
                            {
                                if (audio.GetProperty("isAvailable").GetBoolean())
                                {
                                    videoHlsData += "[\""+video.GetProperty("id").GetString()+"\",\""+audio.GetProperty("id").GetString()+"\"],";
                                    //Console.Write("[\""+video.GetProperty("id").GetString()+"\",\""+audio.GetProperty("id").GetString()+"\"],");
                                }
                            }
                        }

                    }
                    //Console.WriteLine(videoHlsData);
                    videoHlsData = videoHlsData[..^1] + "]}";

                    string apiUrl = "https://nvapi.nicovideo.jp/v1/watch/"+videoId+"/access-rights/hls?actionTrackId="+watchTrackId;
                    if (NicovrcSetting.NicoNico__user_session is not null)
                    {
                        client.DefaultRequestHeaders.Remove("Cookie");
                        client.DefaultRequestHeaders.Add("Cookie", "nicosid=" + nicosid);
                    } else
                    {
                        client.DefaultRequestHeaders.Remove("Cookie");
                        client.DefaultRequestHeaders.Add("Cookie", "nicosid=" + nicosid + "; user_session=" + NicovrcSetting.NicoNico__user_session);
                    }
                    client.DefaultRequestHeaders.Add("X-Access-Right-Key", json.RootElement.GetProperty("data").GetProperty("response").GetProperty("$watchV4").GetProperty("data").GetProperty("media").GetProperty("accessRightKey").GetString());
                    client.DefaultRequestHeaders.Add("X-Frontend-Id", "6");
                    client.DefaultRequestHeaders.Add("X-Frontend-Version", "0");
                    client.DefaultRequestHeaders.Add("X-Niconico-Language", "ja-jp");
                    client.DefaultRequestHeaders.Add("X-Request-With", "https://www.nicovideo.jp");

                    StringContent jsonContent = new(videoHlsData,Encoding.UTF8,"application/json");

                    httpResponseMessage = await client.PostAsync(apiUrl, jsonContent);
                    Console.WriteLine(httpResponseMessage.StatusCode);
                    if (((int)httpResponseMessage.StatusCode) >= 200 && ((int)httpResponseMessage.StatusCode) < 300)
                    {
                        response = await httpResponseMessage.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        return JsonSerializer.Serialize(new ErrorResult("HTTPエラー : " + httpResponseMessage.StatusCode));
                    }
                    //Console.WriteLine(response);

                    json = JsonDocument.Parse(response);
                    result.SetVideoURL(json.RootElement.GetProperty("data").GetProperty("contentUrl").GetString() ?? "");

                    string[] cookie = ["nicosid=\"" + nicosid + "\""];
                    if (NicovrcSetting.NicoNico__user_session is not null)
                    {
                        cookie.Append("user_session=\"" + NicovrcSetting.NicoNico__user_session + "\"");
                    }
                    
                    result.SetVideoAccessCookie(cookie);

                    client.Dispose();
                    //Console.WriteLine(JsonSerializer.Serialize(result));
                    return JsonSerializer.Serialize(result);
                }
                else if (httpResponseMessage.RequestMessage is not null && httpResponseMessage.RequestMessage.RequestUri is not null && httpResponseMessage.RequestMessage.RequestUri.Host == "live.nicovideo.jp")
                {
                    // ニコ生
                    return JsonSerializer.Serialize(new ErrorResult("未実装です。"));
                }
                else
                {
                    return JsonSerializer.Serialize(new ErrorResult("対応していないURLです。"));
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.StackTrace);
                return JsonSerializer.Serialize(new ErrorResult(e.StackTrace ?? "内部エラー"));
            }
        }
    }
}