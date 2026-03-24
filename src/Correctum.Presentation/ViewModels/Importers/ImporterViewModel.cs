using CommunityToolkit.Mvvm.ComponentModel;
using Correctum.Models;

namespace Correctum.ViewModels.Importers;

public abstract class ImporterViewModel : ObservableObject
{
    public abstract string Name { get; }
    public abstract bool CanStart { get; }
    public abstract IAsyncEnumerable<Receipt> ImportAsync(CancellationToken cancellationToken = default);
}
