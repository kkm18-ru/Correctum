using Microsoft.Extensions.Http.Resilience;
using Polly;
using System.Net.Http;

namespace RetailCorrector.Utils.Integrations.Clients;

public abstract class RestClient : IDisposable
{
    protected static readonly HttpMessageHandler Handler = InitHandler();
    protected HttpClient Client { get; }

    protected RestClient(string? baseAddress = null)
    {
        Client = new HttpClient(Handler, false);
        if (baseAddress is not null) Client.BaseAddress = new Uri(baseAddress);
    }

    private static ResilienceHandler InitHandler()
    {
        var retryPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new HttpRetryStrategyOptions
            {
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3
            }).Build();
        var httpHandler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(10)
        };
        return new ResilienceHandler(retryPipeline) { InnerHandler = httpHandler };
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            Client.Dispose();
        }
    }

    ~RestClient() => Dispose(false);
}
