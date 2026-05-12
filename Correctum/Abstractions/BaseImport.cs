using Correctum.Core.Models;

namespace Correctum.Abstractions;

public abstract class BaseImport : BindableBase, IImport
{
    protected readonly static NLog.Logger logger;
    public abstract IAsyncEnumerable<Receipt> ImportAsync(CancellationToken cancellationToken);
    public virtual void OnClosing() { }
}
