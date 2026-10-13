using Correctum.Resources.Locales;
using Correctum.Utils;
using Correctum.Utils.Integrations;
using NLog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Correctum.Windows;

/// <summary>
/// Отображает состояние передачи фискальных документов оператору фискальных данных (ОФД).
/// Показывает количество оставшихся документов, прогресс передачи, её скорость
/// и приблизительное время до завершения.
/// Периодически обновляет данные, полученные от фискального регистратора,
/// до завершения передачи или закрытия окна.
/// </summary>
internal sealed class FiscalExchangeStatusWindow : Window, INotifyPropertyChanged
{
    /// <summary>
    /// Возвращает количество документов, переданных с момента открытия окна.
    /// Вычисляется как разница между начальным и текущим размером буфера.
    /// </summary>
    public uint SentDocumentCount =>
        CurrentUnsentDocumentCount > InitialUnsentDocumentCount ? 0 :
        InitialUnsentDocumentCount - CurrentUnsentDocumentCount;

    /// <summary>
    /// Возвращает время, прошедшее с момента открытия окна.
    /// </summary>
    public TimeSpan Elapsed => DateTime.UtcNow - _initialDateTime;

    /// <summary>
    /// Возвращает процент документов, переданных с момента открытия окна,
    /// относительно начального количества документов в буфере.
    /// </summary>
    public double Percent => SentDocumentCount / InitialUnsentDocumentCount * 100;

    /// <summary>
    /// Возвращает среднюю скорость передачи документов в ОФД
    /// в документах в секунду с момента открытия окна.
    /// </summary>
    public double Speed => Elapsed.TotalSeconds == 0 ? 0 : SentDocumentCount / Elapsed.TotalSeconds;

    /// <summary>
    /// Возвращает приблизительное количество секунд, необходимое
    /// для передачи оставшихся документов при текущей средней скорости.
    /// </summary>
    public double RemainingSeconds => Speed == 0 ? 0 : CurrentUnsentDocumentCount / Speed;

    /// <summary>
    /// Возвращает текущее количество документов, ожидающих передачи в ОФД.
    /// При изменении значения уведомляет подписчиков об изменении свойства
    /// и всех вычисляемых показателей прогресса.
    /// </summary>
    public uint CurrentUnsentDocumentCount { get; private set => SetField(ref field, value); }

    /// <summary>
    /// Возвращает начальное количество документов в буфере на момент
    /// создания окна. Используется в качестве исходного значения
    /// для расчёта прогресса передачи.
    /// </summary>
    public uint InitialUnsentDocumentCount { get; }


    private Task? _task;
    private readonly CancellationTokenSource _cts = new();
    private readonly DateTime _initialDateTime;
    private readonly FptrHandle _driver;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    private FiscalExchangeStatusWindow(FptrHandle driver, DateTime dt, uint count)
    {
        _driver = driver;
        CurrentUnsentDocumentCount = InitialUnsentDocumentCount = count;
        _initialDateTime = dt;
        _logger.Debug(ML.FiscalExchangeStatusWindow_UpdateTemplate,
            _initialDateTime.UnixTimestampSeconds, InitialUnsentDocumentCount);
        UpdateUI();
    }
#pragma warning disable CS8618
    private FiscalExchangeStatusWindow() { }
#pragma warning restore CS8618

    public static void ShowDialogIfExistsUnsentDocuments(FptrHandle driver, bool forceIfExists)
    {
        try
        {
            var dt = DateTime.UtcNow;
            var count = driver.GetUnsentCount();
            if (count > Configuration.Current.Fptr.SizeStack || (forceIfExists && count > 0))
                new FiscalExchangeStatusWindow(driver, dt, count).ShowDialog();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, ML.FiscalExchangeStatusWindow_ErrorUpdate);
        }
    }

    protected override void OnActivated(EventArgs e)
    {
        _task ??= UpdateAsync(_cts.Token);
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
            try
            {
                CurrentUnsentDocumentCount = _driver.GetUnsentCount();
                _logger.Debug(ML.FiscalExchangeStatusWindow_UpdateTemplate,
                    DateTime.UtcNow.UnixTimestampSeconds, CurrentUnsentDocumentCount);
                if (CurrentUnsentDocumentCount == 0) Close();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ML.FiscalExchangeStatusWindow_ErrorUpdate);
            }
            finally
            {
                UpdateUI();
            }
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

    private void UpdateUI()
    {
        OnPropertyChanged(nameof(SentDocumentCount));
        OnPropertyChanged(nameof(Elapsed));
        OnPropertyChanged(nameof(Percent));
        OnPropertyChanged(nameof(Speed));
        OnPropertyChanged(nameof(RemainingSeconds));
    }
}
