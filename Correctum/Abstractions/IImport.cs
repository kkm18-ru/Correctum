using Correctum.Core.Models;
using Correctum.ViewModels.Controls;

namespace Correctum.Abstractions;

public interface IImport
{
    IAsyncEnumerable<Receipt> ImportAsync(MultiProgress progress, CancellationToken cancellationToken);
    void OnClosing() { }
}
