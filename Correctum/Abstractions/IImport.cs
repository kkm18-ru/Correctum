using Correctum.Core.Models;

namespace Correctum.Abstractions;

public interface IImport
{
    IAsyncEnumerable<Receipt> ImportAsync(CancellationToken cancellationToken);
    void OnClosing() { }
}
