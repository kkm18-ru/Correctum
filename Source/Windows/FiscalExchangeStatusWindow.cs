using Correctum.Utils;
using Correctum.Utils.Integrations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Correctum.Windows;

internal sealed class FiscalExchangeStatusWindow
    : Window, 
      INotifyPropertyChanged
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
    public ICommand SkipCommand { get; }

    private readonly CancellationTokenSource _cts = new();
    private Task? _task;
    private readonly FptrHandle _driver;

    public event PropertyChangedEventHandler? PropertyChanged;

    public FiscalExchangeStatusWindow(FptrHandle driver)
    {
        _driver = driver;
        UsedBufferSize = InitialiteBufferSize = driver.GetUnsentCount();
        InitialiteDateTime = DateTime.UtcNow;
        SkipCommand = new RelayCommand(Close);
    }


    protected override void OnActivated(EventArgs e)
    {
        _task = UpdateAsync();
        base.OnActivated(e);
    }

    protected override async void OnClosing(CancelEventArgs e)
    {
        _cts.Cancel();
        try
        {
            if (_task is { })
                await _task;
        }
        catch (OperationCanceledException) { }
        _cts.Dispose();
        base.OnClosing(e);
    }

    private async Task UpdateAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            UsedBufferSize = _driver.GetUnsentCount();
            if (UsedBufferSize == 0) Close(); 
            await Task.Delay(TimeSpan.FromSeconds(1));
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
