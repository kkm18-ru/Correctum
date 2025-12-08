using Microsoft.Extensions.Http.Resilience;
using Polly;
using System.Net.Http;

namespace RetailCorrector.Utils.Integrations.Clients;

internal abstract class APIClient
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
