using Correctum.Core.Models;
using Correctum.UI.Windows;
using Correctum.Utils;

namespace Correctum.Importers;

internal abstract class ImporterBase<TP, TV> : IImporter, ISingleton<TP>
    where TP : ImporterBase<TP, TV>, ISingleton<TP>, new()
    where TV : ImporterView<TV, TP>, new()
{
    public static TP Instance => field ??= new TP();

    public abstract string Name { get; }

    public void Show() => new ImportWindow(new TV()).ShowDialog();
    public abstract IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, CancellationToken cancellationToken);
    public virtual void OnClosing() { }
}