using System.Reflection;
using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;

namespace RetailCorrector.Importers;

public abstract class ImporterBase: FormGrid
{
    public string DisplayName => GetType().GetCustomAttribute<ImporterNameAttribute>()?.Name ?? "Безымянный импортер";
    
    public abstract IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress, Guid sessionId,
        CancellationToken cancellationToken = default);
}