using PuppeteerSharp;

namespace RetailCorrector.Utils.Extensions
{
    public static class PuppeteerExtensions
    {
        extension(CookieParam cookieParam)
        {
            public System.Net.Cookie ToCookie() => new()
            {
                Expires = cookieParam.Expires is null ? DateTime.MaxValue : DateTime.UnixEpoch.AddSeconds(cookieParam.Expires.Value),
                Expired = cookieParam.Expires is not null && DateTime.UnixEpoch.AddSeconds(cookieParam.Expires.Value) <= DateTime.Now,
                Secure = cookieParam.Secure ?? false,
                Name = cookieParam.Name,
                Value = cookieParam.Value,
                Path = cookieParam.Path,
                Domain = cookieParam.Domain,
                HttpOnly = cookieParam.HttpOnly ?? false
            };
        }
    }
}
