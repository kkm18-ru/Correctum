using Correctum.Core.Models;
using Correctum.Utils;

namespace Correctum.Abstractions;

public interface IImport
{
    IAsyncEnumerable<Receipt> ImportAsync(BoundedProgress<uint> progress, CancellationToken cancellationToken);
    void OnClosing() { }
}
