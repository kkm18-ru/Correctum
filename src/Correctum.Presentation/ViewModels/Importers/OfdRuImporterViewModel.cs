using CommunityToolkit.Mvvm.ComponentModel;
using Correctum.Models;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Correctum.ViewModels.Importers;

public sealed partial class OfdRuImporterViewModel : AuthorizableImporterViewModel
{
    [ObservableProperty] private string m_login = string.Empty;
    [ObservableProperty] private string m_password = string.Empty;
    [ObservableProperty] private DateTime m_beginPeriod = DateTime.Now;
    [ObservableProperty] private DateTime m_endPeriod = DateTime.Now;
    public ObservableCollection<object> AvailableDevices { get; } = [];
    public ObservableCollection<object> SelectedDevices { get; } = [];

    public override string Name => "ОФД.ру (Логин-пароль)";
    public override bool CanStart => Authorized && SelectedDevices.Count > 0;

    //private readonly OfdRuClient m_client = new();

    public OfdRuImporterViewModel()
    {
        SelectedDevices.CollectionChanged += (_, _) => OnPropertyChanged(nameof(CanStart));
    }

    protected override async Task InternalAuthAsync()
    {

    }

    public override async IAsyncEnumerable<Receipt> ImportAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield break;
    }
}
