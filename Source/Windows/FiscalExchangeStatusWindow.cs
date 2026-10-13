using Correctum.Resources.Locales;
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
    public double SendedDocument => InitialiteBufferSize - UsedBufferSize;

    /// <summary>
    /// Возвращает время, прошедшее с момента открытия окна.
    /// </summary>
    public TimeSpan Elapsed => DateTime.UtcNow - InitialiteDateTime;

    /// <summary>
    /// Возвращает процент документов, переданных с момента открытия окна,
    /// относительно начального количества документов в буфере.
    /// </summary>
    public double Percent => SendedDocument / InitialiteBufferSize * 100;

    /// <summary>
    /// Возвращает среднюю скорость передачи документов в ОФД
    /// в документах в секунду с момента открытия окна.
    /// </summary>
    public double Speed => SendedDocument / Elapsed.TotalSeconds;

    /// <summary>
    /// Возвращает приблизительное количество секунд, необходимое
    /// для передачи оставшихся документов при текущей средней скорости.
    /// </summary>
    public double RemainingSeconds => UsedBufferSize / Speed;

    /// <summary>
    /// Возвращает текущее количество документов, ожидающих передачи в ОФД.
    /// При изменении значения уведомляет подписчиков об изменении свойства
    /// и всех вычисляемых показателей прогресса.
    /// </summary>
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

    /// <summary>
    /// Возвращает начальное количество документов в буфере на момент
    /// создания окна. Используется в качестве исходного значения
    /// для расчёта прогресса передачи.
    /// </summary>
    public uint InitialiteBufferSize { get; }

    /// <summary>
    /// Возвращает дату и время создания окна в формате UTC.
    /// Используется для расчёта продолжительности передачи.
    /// </summary>
    public DateTime InitialiteDateTime { get; }

    private readonly CancellationTokenSource _cts = new();
    private Task? _task;
    private readonly FptrHandle _driver;

    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Инициализирует окно состояния фискального обмена и получает
    /// начальное количество документов, ожидающих передачи в ОФД.
    /// </summary>
    /// <param name="driver">
    /// Экземпляр драйвера фискального регистратора, используемый
    /// для получения количества неотправленных документов.
    /// </param>
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
