using CommunityToolkit.Mvvm.ComponentModel;
using Correctum.ApiClients.OfdRuTokenClient;
using Correctum.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Correctum.ViewModels.Importers;

public sealed partial class OfdRuTokenImporterViewModel : AuthorizableImporterViewModel
{
    [ObservableProperty] private string m_authToken = string.Empty;
    [ObservableProperty] private DateTime m_beginPeriod = DateTime.Now;
    [ObservableProperty] private DateTime m_endPeriod = DateTime.Now;
    public ObservableCollection<OfdRuTokenKkt> AvailableDevices { get; } = [];
    public ObservableCollection<OfdRuTokenKkt> SelectedDevices { get; } = [];

    public override string Name => "ОФД.ру (Токен)";
    public override bool CanStart => Authorized && SelectedDevices.Count > 0;

    private readonly OfdRuTokenClient m_client = new();

    public OfdRuTokenImporterViewModel()
    {
        SelectedDevices.CollectionChanged += (_, _) => OnPropertyChanged(nameof(CanStart));
    }

    protected override async Task InternalAuthAsync()
    {
        SelectedDevices.Clear();
        AvailableDevices.Clear();
        await foreach (var kkt in m_client.EnumerateKktsAsync(AuthToken))
            AvailableDevices.Add(kkt);
    }

    public override async IAsyncEnumerable<Receipt> ImportAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield break;
    }
}
