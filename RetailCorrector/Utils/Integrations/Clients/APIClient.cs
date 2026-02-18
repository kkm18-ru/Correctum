using Microsoft.Extensions.Http.Resilience;
using Polly;
using System.Net.Http;

namespace RetailCorrector.Utils.Integrations.Clients;

public abstract class ResilienceApiClient
{
    protected static readonly HttpMessageHandler Handler = InitHandler();
    
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
}

public abstract class ApiClient(string host): ResilienceApiClient, IDisposable
{
    protected readonly HttpClient Client = new(Handler, disposeHandler: false) { BaseAddress = new Uri(host) };
    private bool _disposed;


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        Client.Dispose();
        if(disposing)
            _disposed = true;
    }
}
