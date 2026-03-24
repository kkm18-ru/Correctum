using CommunityToolkit.Mvvm.ComponentModel;
using Correctum.ApiClients.SabyClient;
using Correctum.Models;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Correctum.ViewModels.Importers;

public sealed partial class SabyImporterViewModel : ImporterViewModel
{
    [ObservableProperty] private string m_login = string.Empty;
    [ObservableProperty] private string m_password = string.Empty;
    [ObservableProperty] private bool m_asFranchise = false;
    [ObservableProperty] private DateTime m_beginPeriod = DateTime.Now;
    [ObservableProperty] private DateTime m_endPeriod = DateTime.Now;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(CanStart))] private bool m_authorized = false;
    [ObservableProperty] private bool m_authorizing = false;
    public ObservableCollection<object> AvailableDevices { get; } = [];
    public ObservableCollection<object> SelectedDevices { get; } = [];

    public override string Name => "SabyOFD (СБИС)";
    public override bool CanStart => Authorized && SelectedDevices.Count > 0;

    private readonly SabyClient m_client = new();

    public SabyImporterViewModel()
    {
        SelectedDevices.CollectionChanged += (_, _) => OnPropertyChanged(nameof(CanStart));
    }

    public async void Auth() { }

    public override async IAsyncEnumerable<Receipt> ImportAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield break;
    }
}
