using Correctum.Core.Models;
using NLog;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations.Clients;
using System.Windows;
using System.Windows.Input;

namespace RetailCorrector.Importers;

internal partial class FirstOfd : ImporterBase<FirstOfd, FirstOfd.View>
{
    public override string Name => "1-ОФД";
    public ICommand AuthCommand { get; }

    private readonly FirstOfdClient Client;

    private readonly static Logger Logger = LogManager.GetCurrentClassLogger();

    public FirstOfd()
    {
        Client = new FirstOfdClient();
        AuthCommand = new RelayCommand(AuthAsync, _ => !(Authorizing || string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password)));
    }

    protected internal async void AuthAsync(object? _)
    {
        try
        {
            Authorizing = true;
            await Client.AuthAsync(Login, Password);
            Devices.Clear();
            foreach(var placeId in await Client.GetRetailPlaceIdsAsync())
                foreach (var kkt in await Client.GetKktsAsync(placeId))
                    Devices.Add(kkt);
            IsAuthorized = true;
        }
        catch(Exception ex)
        {
            Logger.Error(ex, "Не удалось авторизовать в 1-ОФД!");
            MessageBox.Show(ex.ToString(), "Ошибка авторизации");
            IsAuthorized = false;
        }
        finally
        {
            Authorizing = false;
        }
    }

    public override async IAsyncEnumerable<Receipt> Import(BoundedProgress<uint> progress, CancellationToken cancellationToken)
    {
        List<FirstOfdTransaction> transactions = [];

        foreach(var kkt in SelectedDevices)
        {
            foreach (var fn in await Client.GetFiscalDrivesAsync(kkt, cancellationToken))
            {
                if (fn.activationDate is null || fn.expireDate is null) continue;
                if (DateOnly.FromDateTime(fn.activationDate!.Value) > DateRange.EndDate) continue;
                if (DateOnly.FromDateTime(fn.closeArchiveDate ?? fn.expireDate.Value) < DateRange.BeginDate) continue;
                var fromDate = DateRange.BeginDate.ToDateTime(TimeOnly.MinValue);
                var toDate = DateRange.EndDate.ToDateTime(TimeOnly.MaxValue);
                var size = Configuration.Instance.HttpClient.PageSize;
                var enumerable = Client.GetTransactionsAsync(kkt, fromDate, toDate, fn, size, cancellationToken);
                await foreach(var tr in enumerable.WithCancellation(cancellationToken))
                    transactions.AddRange(tr);
            }
        }
        progress.ReportMaximum((uint)transactions.Count);

        foreach(var tr in transactions)
        {
            yield return await Client.GetReceiptAsync(tr, cancellationToken);
            progress.ReportIncrement();
        }
    }

    public override void OnClosing()
    {
        Client.Dispose();
        base.OnClosing();
    }
}
