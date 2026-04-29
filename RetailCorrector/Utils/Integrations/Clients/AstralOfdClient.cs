using PuppeteerSharp;

namespace RetailCorrector.Utils.Integrations.Clients;

internal sealed class AstralOfdClient(string host = "https://ofd.astralnalog.ru") : ApiClient<AstralOfdClient>(host)
{
    public async Task AuthByEmailAsync(string email, string password, string host = "ofd.astralnalog.ru", CancellationToken cancellationToken = default)
    {
        var options = new LaunchOptions
        {
            Headless = true,
            ExecutablePath = Configuration.Instance.HttpClient.HeadlessBrowserPath
        };
        cancellationToken.ThrowIfCancellationRequested();
        using var browser = await Puppeteer.LaunchAsync(options);
        cancellationToken.ThrowIfCancellationRequested();
        using var page = await browser.NewPageAsync();
        cancellationToken.ThrowIfCancellationRequested();
        await page.GoToAsync($"http://{host}/lk/");
        cancellationToken.ThrowIfCancellationRequested();
        await page.ClickAsync("a[data-test='authLoginTypeToggle']");
        cancellationToken.ThrowIfCancellationRequested();
        await page.TypeAsync("input[name='email']", email);
        cancellationToken.ThrowIfCancellationRequested();
        await page.TypeAsync("input[name='password']", password);
        cancellationToken.ThrowIfCancellationRequested();
        await page.ClickAsync("button[type='submit']");
        cancellationToken.ThrowIfCancellationRequested();
        await page.WaitForResponseAsync(resp => resp.Url.EndsWith("users.getMe"));
        cancellationToken.ThrowIfCancellationRequested();
        Cookies.Add((await page.GetCookiesAsync()).First(c => c.Name == "connect.sid").ToCookie());
    }
}
