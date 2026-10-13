using Correctum.Resources.Locales;
using Correctum.Utils.Integrations;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Correctum.Windows;

internal sealed class FiscalExchangeStatusWindow : Window, INotifyPropertyChanged
{
    public double SendedDocument => InitialiteBufferSize - UsedBufferSize;
    public TimeSpan Elapsed => DateTime.UtcNow - InitialiteDateTime;
    public double Percent => SendedDocument / InitialiteBufferSize * 100;
    public double Speed => SendedDocument / Elapsed.TotalSeconds;
    public double RemainingSeconds => UsedBufferSize / Speed;

    public uint UsedBufferSize
    {
        get; private set
        {
            if (SetField(ref field, value))
            {
                OnPropertyChanged(nameof(SendedDocument));
                OnPropertyChanged(nameof(Elapsed));
                OnPropertyChanged(nameof(Percent));
                OnPropertyChanged(nameof(Speed));
                OnPropertyChanged(nameof(RemainingSeconds));
            }
        }
    }

    public uint InitialiteBufferSize { get; }
    public DateTime InitialiteDateTime { get; }

    private readonly CancellationTokenSource _cts = new();
    private Task? _task;
    private readonly FptrHandle _driver;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public event PropertyChangedEventHandler? PropertyChanged;

    public FiscalExchangeStatusWindow(FptrHandle driver)
    {
        _driver = driver;
        UsedBufferSize = InitialiteBufferSize = driver.GetUnsentCount();
        InitialiteDateTime = DateTime.UtcNow;
        _logger.Debug(ML.FiscalExchangeStatusWindow_UpdateTemplate, 
            InitialiteDateTime.UnixTimestampSeconds, InitialiteBufferSize);
    }

    protected override void OnActivated(EventArgs e)
    {
        _task = UpdateAsync(_cts.Token);
        base.OnActivated(e);
    }

    protected override async void OnClosing(CancelEventArgs e)
    {
        _logger.Info(ML.FiscalExchangeStatusWindow_Closing);
        try
        {
            base.OnClosing(e);
            await _cts.CancelAsync();
            if (_task is { })
                await _task;
        }
        catch (OperationCanceledException ex)
        {
            _logger.Debug(ex, ML.FiscalExchangeStatusWindow_CanceledTask);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ML.FiscalExchangeStatusWindow_ErrorCancelTask);
        }
        finally
        {
            _cts.Dispose();
        }
    }

    private async Task UpdateAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            UsedBufferSize = _driver.GetUnsentCount();
            _logger.Debug(ML.FiscalExchangeStatusWindow_UpdateTemplate, 
                DateTime.UtcNow.UnixTimestampSeconds, UsedBufferSize);
            if (UsedBufferSize == 0) Close(); 
            await Task.Delay(TimeSpan.FromSeconds(1), ct);
        }
    }

    private void OnPropertyChanged(string? propertyName)
    {
        PropertyChanged?.Invoke(this, new(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
