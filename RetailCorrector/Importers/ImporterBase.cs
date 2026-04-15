using RetailCorrector.Data;
using Correctum.Core.Models;
using RetailCorrector.UI.Windows;
using RetailCorrector.Utils;

namespace RetailCorrector.Importers;

internal abstract class ImporterBase<TP, TV> : IImporter, ISingleton<TP>
    where TP : ImporterBase<TP, TV>, ISingleton<TP>, new()
    where TV : ImporterView<TV, TP>, new()
{
    public static TP Instance => field ??= new TP();

    public abstract string Name { get; }
    protected ImportSession parseInfo;
    public abstract ImportSession GenerateInfo();

    public void Show() => new ImportWindow(new TV()).ShowDialog();
    public abstract IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, CancellationToken cancellationToken);
    public virtual void OnClosing() { }
}