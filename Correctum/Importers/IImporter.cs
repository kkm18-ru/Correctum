using Correctum.Core.Models;
using Correctum.Utils;

namespace Correctum.Importers;

internal interface IImporter
{
    string Name { get; }
    void Show();
    IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, CancellationToken cancellationToken);
    void OnClosing();
}
