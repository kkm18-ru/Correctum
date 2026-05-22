using Correctum.Core.Models;
using Correctum.Utils;

namespace Correctum.Abstractions;

public abstract class BaseImport : BindableBase, IImport
{
    protected readonly static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
    public abstract IAsyncEnumerable<Receipt> ImportAsync(BoundedProgress<uint> progress, CancellationToken cancellationToken);
    public virtual void OnClosing() { }
}
