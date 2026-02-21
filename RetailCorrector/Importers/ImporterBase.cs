using System.Reflection;
using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;

namespace RetailCorrector.Importers;

public abstract class ImporterBase: FormGrid
{
    public string DisplayName => GetType().GetCustomAttribute<ImporterNameAttribute>()?.Name ?? "Безымянный импортер";
    public abstract bool CanStartClick { get; }
    
    public abstract IAsyncEnumerable<Receipt> ImportAsync(VisualProgress progress, Guid sessionId,
        CancellationToken cancellationToken = default);

    protected new bool SetField<T>(ref T field, T value, string? propertyName = null)
    {
        if (!base.SetField(ref field, value, propertyName)) return false;
        OnPropertyChanged(nameof(CanStartClick));
        return true;
    }
}