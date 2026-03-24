using System.Net;

namespace Correctum.Extensions;

public static class CookieCollectionExtensions
{
    extension(CookieCollection cookies)
    {
        public void SetOrAdd(string key, string? value)
        {
            if (cookies[key] is { } cookie) cookies.Remove(cookie);
            if(value is not null) cookies.Add(new Cookie(key, value));
        }
    }
}
