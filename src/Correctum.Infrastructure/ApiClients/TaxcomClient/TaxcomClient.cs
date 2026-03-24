using Correctum.Models;

namespace Correctum.ApiClients.TaxcomClient;

public sealed class TaxcomClient(string host = "https://api-lk-ofd.taxcom.ru/") : BaseApiClient(host)
{/*
    public async Task AuthAsync(string login, string password, string? agreement, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = "/API/v2/Login";
    }

    public async IAsyncEnumerable<string> EnumerateOutletIdsAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = $"/API/v2/OutletList?&pn={pn}&ps={ps}";
    }

    public async IAsyncEnumerable<string> EnumerateKktRegNumbersAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = $"/API/v2/KKTList?id={id}&pn={pn}&ps={ps}";
    }

    public async IAsyncEnumerable<string> EnumerateDriveSerialAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = $"/API/v2/FnHistory?kktRegNumber={kktRegNumber}";
    }

    public async IAsyncEnumerable<string> EnumerateSessionIdsAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = $"/API/v2/ShiftList?fn={fn}&begin={begin}&end={end}&pn={pn}&ps={ps}";
    }

    public async IAsyncEnumerable<string> EnumerateDocumentNumbersAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = $"/API/v2/DocumentList?fn={fn}&shift={shift}&pn={pn}&ps={ps}";
    }

    public async Task<Receipt> FetchReceiptAsync(string fn, uint fd, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(m_disposed, this);
        var uri = $"/API/v2/DocumentInfo?fn={fn}&fd={fd}";
        

    }*/
}
