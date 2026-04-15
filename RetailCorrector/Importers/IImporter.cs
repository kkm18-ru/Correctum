using Correctum.Core.Models;
using RetailCorrector.Utils;

namespace RetailCorrector.Importers;

internal interface IImporter
{
    string Name { get; }
    void Show();
    IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, CancellationToken cancellationToken);
    void OnClosing();
}
