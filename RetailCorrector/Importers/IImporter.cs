using RetailCorrector.Data;
using RetailCorrector.Utils;

namespace RetailCorrector.Importers;

internal interface IImporter
{
    string Name { get; }
    void Show();
    ImportSession GenerateInfo();
    IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, CancellationToken cancellationToken);
    void OnClosing();
}
