using Correctum.Core.Models;
using Correctum.ViewModels.Controls;

namespace Correctum.Abstractions;

public abstract class BaseImport : BindableBase, IImport
{
    protected readonly static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    public abstract IAsyncEnumerable<Receipt> ImportAsync(MultiProgress progress, CancellationToken cancellationToken);
    public virtual void OnClosing() { }
}
