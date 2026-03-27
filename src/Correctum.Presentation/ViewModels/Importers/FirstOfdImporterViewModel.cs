using CommunityToolkit.Mvvm.ComponentModel;
using Correctum.ApiClients.FirstOfdClient;
using Correctum.ApiClients.FirstOfdClient.Models;
using Correctum.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Correctum.ViewModels.Importers;

public sealed partial class FirstOfdImporterViewModel : AuthorizableImporterViewModel
{
    [ObservableProperty] private string m_login = string.Empty;
    [ObservableProperty] private string m_password = string.Empty;
    [ObservableProperty] private DateTime m_beginPeriod = DateTime.Now;
    [ObservableProperty] private DateTime m_endPeriod = DateTime.Now;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(CanStart))] private bool m_authorized = false;
    [ObservableProperty] private bool m_authorizing = false;
    public ObservableCollection<FirstOfdKkt> AvailableDevices { get; } = [];
    public ObservableCollection<FirstOfdKkt> SelectedDevices { get; } = [];

    public override string Name => "Такском";
    public override bool CanStart => Authorized && SelectedDevices.Count > 0;

    private readonly FirstOfdClient m_client = new();

    public FirstOfdImporterViewModel()
    {
        SelectedDevices.CollectionChanged += (_, _) => OnPropertyChanged(nameof(CanStart));
    }

    public override async IAsyncEnumerable<Receipt> ImportAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        yield break;
    }

    protected override async Task InternalAuthAsync()
    {
        await m_client.AuthAsync(Login, Password);
        foreach(var placeId in await m_client.GetPlaceIdsAsync())
        {
            foreach (var kkt in await m_client.GetKktsAsync(placeId))
                AvailableDevices.Add(kkt);
        }
    }
}
