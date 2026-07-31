using PuppeteerSharp;

namespace Correctum.Utils;

public static class PuppeteerHelper
{
    public static Task<IBrowser> OpenBrowser() => Puppeteer.LaunchAsync(new()
    {
        Headless = false,
        ExecutablePath = Configuration.Current.Http.WebBrowserPath,
        Protocol = Configuration.Current.Http.WebBrowserProtocol,
        Browser = Configuration.Current.Http.WebBrowserType,
        EnableExtensions = new EnableExtensionsOption(false),
        DefaultViewport = new ViewPortOptions()
        {
            IsMobile = false,
            IsLandscape = false
        },
        IgnoredDefaultArgs = ["--enable-automation"],
        IssuesEnabled = false,
        Args = ["--kiosk"],
        AcceptInsecureCerts = false,
        HeadlessMode = HeadlessMode.False
    });
}
