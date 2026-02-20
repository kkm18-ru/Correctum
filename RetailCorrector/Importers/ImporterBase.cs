using RetailCorrector.Data;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;

namespace RetailCorrector.Importers;

public abstract class ImporterBase: FormGrid
{
    public abstract IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress,
        CancellationToken cancellationToken = default);
}