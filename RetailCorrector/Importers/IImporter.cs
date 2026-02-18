using RetailCorrector.Data;

namespace RetailCorrector.Importers;

internal interface IImporter
{
    string Name { get; }
    void Show();
    ImportSession GenerateInfo();
    IAsyncEnumerable<Receipt> Import(IProgress<long> progress, CancellationToken cancellationToken);
    void OnClosing();
}
