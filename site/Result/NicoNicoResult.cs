using System.Text.Json.Serialization;

namespace net.nicovrc.dev.site.Result
{
    public class NicoNicoResult
    {
        [JsonInclude]
        private string URL = "";
        [JsonInclude]
        private string Title = "";
        [JsonInclude]
        private string Description = "";
        [JsonInclude]
        private string[] Tags = [];
        [JsonInclude]
        private string Thumbnail = "";
        [JsonInclude]
        private long ViewCount = 0;
        [JsonInclude]
        private long CommentCount = 0;
        [JsonInclude]
        private long MyListCount = 0;
        [JsonInclude]
        private long LikeCount = 0;
        [JsonInclude]
        private long Duration = 0;
        [JsonInclude]
        private string? VideoURL;
        [JsonInclude]
        private string[]? VideoAccessCookie;
        [JsonInclude]
        private string? LiveURL;
        [JsonInclude]
        private string[]? LiveAccessCookie;

        public void SetURL(string url)
        {
            URL = url;
        }

        public string GetURL()
        {
            return URL;
        }

        public void SetTitle(string title)
        {
            Title = title;
        }

        public string GetTitle()
        {
            return Title;
        }

        public void SetDescription(string description)
        {
            Description = description;
        }

        public string GetDescription()
        {
            return Description;
        }

        public void SetTags(string[] tags)
        {
            Tags = tags;
        }

        public string[] GetTags()
        {
            return Tags;
        }

        public void AddTag(string tag)
        {
            Tags.Append(tag);
        }

        public void SetThumbnail(string thumbnail)
        {
            Thumbnail = thumbnail;
        }

        public string GetThumbnail()
        {
            return Thumbnail;
        }

        public void SetViewCount(long viewCount)
        {
            ViewCount = viewCount;
        }

        public long GetViewCount()
        {
            return ViewCount;
        }

        public void SetCommentCount(long commentCount)
        {
            CommentCount = commentCount;
        }

        public long GetCommentCount()
        {
            return CommentCount;
        }

        public void SetMyListCount(long myListCount)
        {
            MyListCount = myListCount;
        }

        public long GetMyListCount()
        {
            return MyListCount;
        }

        public void SetLikeCount(long likeCount)
        {
            LikeCount = likeCount;
        }

        public long GetLikeCount()
        {
            return LikeCount;
        }

        public void SetDuration(long duration)
        {
            Duration = duration;
        }

        public long GetDuration()
        {
            return Duration;
        }

        public void SetVideoURL(string videoUrl)
        {
            VideoURL = videoUrl;
        }

        public string? GetVideoURL()
        {
            return VideoURL;
        }

        public void SetVideoAccessCookie(string[] videoAccessCookie)
        {
            VideoAccessCookie = videoAccessCookie;
        }

        public string[]? GetVideoAccessCookie()
        {
            return VideoAccessCookie;
        }

        public void AddVideoAccessCookie(string videoAccessCookieValue)
        {
            if (VideoAccessCookie is null)
            {
                VideoAccessCookie = [videoAccessCookieValue];
            }
            else
            {
                VideoAccessCookie.Append(videoAccessCookieValue);
            }
        }

        public void SetLiveURL(string liveUrl)
        {
            LiveURL = liveUrl;
        }

        public string? GetLiveURL()
        {
            return LiveURL;
        }

        public void SetLiveAccessCookie(string[] liveAccessCookie)
        {
            LiveAccessCookie = liveAccessCookie;
        }

        public string[]? GetLiveAccessCookie()
        {
            return LiveAccessCookie;
        }

        public void AddLiveAccessCookie(string liveAccessCookieValue)
        {
            if (LiveAccessCookie is null)
            {
                LiveAccessCookie = [liveAccessCookieValue];
            }
            else
            {
                LiveAccessCookie.Append(liveAccessCookieValue);
            }
        }
    }
}