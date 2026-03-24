using Microsoft.Extensions.Http.Resilience;
using Polly;
using Correctum.Configurations;
using System.Net;

namespace Correctum.ApiClients;

public abstract class BaseApiClient: IDisposable
{
    protected CookieCollection Cookies => CookieContainer.GetCookies(Host);

    protected readonly HttpClient Client;
    protected bool m_disposed;

    private readonly CookieContainer CookieContainer;
    private readonly Uri Host;

    protected BaseApiClient(string host, bool needCookieContainer = false)
    {
        if(needCookieContainer) CookieContainer = new CookieContainer();
        Host = new Uri(host);
        Client = new(InitHandler(CookieContainer), disposeHandler: true) { BaseAddress = Host };
    }

    private static ResilienceHandler InitHandler(CookieContainer? cookieContainer)
    {
        var retryPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new HttpRetryStrategyOptions
            {
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3
            }).AddTimeout(new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(Configuration.Current.HttpClient.Timeout),
            }).Build();
        var httpHandler = new SocketsHttpHandler();
        if (cookieContainer is null)
            httpHandler.CookieContainer = cookieContainer;
        return new ResilienceHandler(retryPipeline) { InnerHandler = httpHandler };
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (m_disposed) return;
        Client.Dispose();
        if (disposing)
            m_disposed = true;
    }
}
