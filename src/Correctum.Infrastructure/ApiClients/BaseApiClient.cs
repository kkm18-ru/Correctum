using Microsoft.Extensions.Http.Resilience;
using Polly;
using Correctum.Configurations;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json;
using System.Net.Http.Json;

namespace Correctum.ApiClients;

public abstract class BaseApiClient: IDisposable
{
    protected HttpClient client { get; }
    protected bool disposed;

    private readonly static HttpMessageHandler handler = InitHandler();

    protected BaseApiClient(string hostAddress)
    {
        client = new(handler, disposeHandler: false) { BaseAddress = new(hostAddress) };
        disposed = false;
    }

    private static ResilienceHandler InitHandler()
    {
        var retryPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
            .AddRetry(new HttpRetryStrategyOptions
            {
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 3
            })
            .AddTimeout(new HttpTimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(Configuration.Current.HttpClient.Timeout),
            })
            .Build();
        var innerHandler = new SocketsHttpHandler { CookieContainer = new() };
        return new ResilienceHandler(retryPipeline) { InnerHandler = innerHandler };
    }

    protected async Task<TResponse?> GetAsync<TException, TResponse>(
        string address, 
        JsonTypeInfo<TException> ctxException, 
        JsonTypeInfo<TResponse> ctxResponse, 
        CancellationToken cancellationToken)
        where TException: Exception
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        using var respMessage = await client.GetAsync(address, cancellationToken).ConfigureAwait(false);
        await using var stream = await respMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        if (!respMessage.IsSuccessStatusCode)
            throw (await JsonSerializer.DeserializeAsync(stream, ctxException, cancellationToken).ConfigureAwait(false)) 
                ?? new Exception("Неизвестная ошибка");
        return await JsonSerializer.DeserializeAsync(stream, ctxResponse, cancellationToken).ConfigureAwait(false);
    }

    protected async Task<TResponse?> PostAsync<TRequest, TException, TResponse>(
        string address, 
        TRequest reqContent, 
        JsonTypeInfo<TRequest> ctxRequest, 
        JsonTypeInfo<TException> ctxException, 
        JsonTypeInfo<TResponse> ctxResponse, 
        CancellationToken cancellationToken)
            where TException: Exception
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        using var respMessage = await client.PostAsJsonAsync(address, reqContent, ctxRequest, cancellationToken).ConfigureAwait(false);
        await using var stream = await respMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        if (!respMessage.IsSuccessStatusCode)
            throw (await JsonSerializer.DeserializeAsync(stream, ctxException, cancellationToken).ConfigureAwait(false)) 
                ?? new Exception("Неизвестная ошибка");
        return await JsonSerializer.DeserializeAsync(stream, ctxResponse, cancellationToken).ConfigureAwait(false);
    }

    protected async Task PostAsync<TRequest, TException>(
        string address, 
        TRequest reqContent, 
        JsonTypeInfo<TRequest> ctxRequest, 
        JsonTypeInfo<TException> ctxException, 
        CancellationToken cancellationToken)
            where TException: Exception
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        using var respMessage = await client.PostAsJsonAsync(address, reqContent, ctxRequest, cancellationToken).ConfigureAwait(false);
        await using var stream = await respMessage.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        if (respMessage.IsSuccessStatusCode) return;
        throw (await JsonSerializer.DeserializeAsync(stream, ctxException, cancellationToken).ConfigureAwait(false)) 
            ?? new Exception("Неизвестная ошибка");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;
        client.Dispose();
        if (disposing)
            disposed = true;
    }
}
