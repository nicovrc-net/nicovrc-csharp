using System;

namespace net.nicovrc.dev.site;

public interface Site
{
    public Task<URLResult> Get(string url);
    public Task<string> GetJson(string url);

}
