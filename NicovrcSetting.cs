using System.Net;

namespace net.nicovrc.dev
{
    public class NicovrcSetting
    {
        public static string Version = "0.1.0-alpha";

        public static string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:159.0) Gecko/20100101 Firefox/159.0 nicovrc-net/" + NicovrcSetting.Version;

        public static string? NicoNico__nicosid = null;
        public static string? NicoNico__user_session = null;

    }
}