using Correctum.Core.Enums;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Enums;
using Correctum.Exceptions;
using Correctum.UI.Windows.Types;
using Correctum.Utils;
using Correctum.Utils.Integrations;
using Correctum.Validators;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace Correctum.UI.Windows;

public partial class FptrWindow : DialogWindow, INotifyPropertyChanged
{
    public int CorrectionType { get; set => SetField(ref field, value); }
    public string Act { get; set => SetField(ref field, value); }
    public bool IsInternet { get; set => SetField(ref field, value); }
    public int CurrentIndex { get; private set => SetField(ref field, value); }
    public int SettingIndex { get; set => SetField(ref field, value); }

    public bool CanSettings => State == ConductingReceiptState.Idle;
    public bool CanDisconnected => State != ConductingReceiptState.Running;
    public bool CanConfigDriver => State == ConductingReceiptState.Disconnected;
    public bool CanRun => State != 0 && State < ConductingReceiptState.TestConducting;
    public string RunButtonText => State == ConductingReceiptState.Running ? "Пауза" : "Запуск";
    public string ConnectionButtonText => State == ConductingReceiptState.Disconnected ? "Подключиться" : "Отключиться";
    private readonly static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

    private ConductingReceiptState State
    {
        get;
        set
        {
            if(!SetField(ref field, value)) return;
            OnPropertyChanged(nameof(CanSettings));
            OnPropertyChanged(nameof(CanDisconnected));
            OnPropertyChanged(nameof(CanRun));
            OnPropertyChanged(nameof(CanConfigDriver));
            OnPropertyChanged(nameof(RunButtonText));
            OnPropertyChanged(nameof(ConnectionButtonText));
        }
    }

    private CancellationTokenSource _cancelSource = null!;
    private readonly FptrHandle _handle;
    
    public FptrWindow()
    {
        Act = "";
        _handle = new FptrHandle(); 
        CurrentIndex = 0;
        InitializeComponent();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (_handle.IsInvalid) return;
        _handle.CancelReceipt();
        _handle.Disconnect();
        _handle.Dispose();
    }

    private void ShowSettingsDriver(object sender, RoutedEventArgs e)
    {
        do if (!_handle.ShowProperties(this)) return;
        while (IsAutoDetectKkt && NotifyAutoDetectKkt());
    }

    private bool IsAutoDetectKkt => 
        _handle.GetSetting("Port") == "1" && _handle.GetSetting("UsbDevicePath") is "auto" or "";

    private static bool NotifyAutoDetectKkt()
    {
        const string text = 
            "У вас выбрано подключение к ККТ с автоматическим выбором устройства. " +
            "Данная настройка не рекомендуется при одновременной работе с несколькими ККТ Atol!\n\n" +
            "Открыть повторно настройку драйвера ККТ?";
        const MessageBoxButton button = MessageBoxButton.YesNo;
        const MessageBoxImage icon = MessageBoxImage.Warning;
        return MessageBox.Show(text, "", button, icon) == MessageBoxResult.Yes;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    
    private void OnPropertyChanged(string? propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
    
    private bool ConductingReceipt(Receipt data, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();
        OpenReceiptArgs openArgs = new(IsInternet, Act, CorrectionType);
        if (!_handle.OpenReceipt(data, openArgs)) return false;

        foreach (var item in data.Items)
        {
            token.ThrowIfCancellationRequested();
            if (!_handle.AddPosition(item)) return false;
        }
        
        token.ThrowIfCancellationRequested();
        if (!_handle.SetTotal(data.TotalSum!.Value)) return false;
        
        token.ThrowIfCancellationRequested();
        if (!_handle.Payment(data)) return false;
        
        token.ThrowIfCancellationRequested();
        return _handle.CloseReceipt();
    }

    private string CaptionError => $"Произошла ошибка (Код: {_handle.ErrorCode})";

    private bool ConnectKkt()
    {
        if (!_handle.Connect())
        {
            MessageBox.Show("Не удалось подключиться к фискальному регистратору", 
                CaptionError, MessageBoxButton.OK, MessageBoxImage.Hand);
            return false;
        }

        var problems = CheckDevice.SearchProblemsInKkt(_handle);
        return problems == AtolKktProblems.None || ShowAtolProblems(problems);
    }

    private bool ShowAtolProblems(AtolKktProblems problems)
    {
        var hasError = ((uint)problems & 0xffff0000u) != 0;
        ReadOnlySpan<(AtolKktProblems problem, string locale)> localeProblems = new[]
        {
            (AtolKktProblems.AdditionalPrintExpired, "Лицензия №17 просрочена"),
            (AtolKktProblems.AdditionalPrintNoFound, "Лицензия №17 не введена"),
            (AtolKktProblems.ConfigurationOutdated,
                $"Модуль конфигурации устарел (v.{_handle.ConfigurationVersion})"),
            (AtolKktProblems.ModelKktUnknown, "Не удалось определить модель ККТ"),
            (AtolKktProblems.CheckKktUnknown, "Не удалось выполнить проверку ККТ"),
            (AtolKktProblems.CheckKktError, "Проверка ККТ завершилась с ошибкой"),
            (AtolKktProblems.CheckKktNeedRegister, "Не завершена регистрация ККТ"),
            (AtolKktProblems.ModelFnUnknown, "Не удалось определить исполнение ФН"),
            (AtolKktProblems.CheckFnUnknown, "Не удалось выполнить проверку ФН"),
            (AtolKktProblems.CheckFnError, "Проверка ФН завершилась с ошибкой"),
            (AtolKktProblems.CheckFnNeedRegister, "Не завершена регистрация ФН"),
            (AtolKktProblems.CheckKktNoCreated, "ККТ не найден в реестре произведенных ККТ"),
            (AtolKktProblems.CheckFnNoCreated, "ФН не найден в реестре произведенных ФН"),
            (AtolKktProblems.CheckFnArchived, "В реестре ФН считается закрытым")
        };
        
        var image = hasError ? MessageBoxImage.Error : MessageBoxImage.Warning;
        const string caption = "Найдены проблемы ККТ";

        var text = new StringBuilder("Вовремя проверки ККТ и ФН были обнаружены следующие проблемы:\n");
        foreach(var item in localeProblems)
            if(problems.HasFlag(item.problem)) text.Append($"- {item.locale}\n");
        
        MessageBox.Show(text.ToString(), caption, 0, image);
        return !hasError;
    }

    private void Finish()
    {
        _handle.CancelReceipt();
        _cancelSource.Dispose();
        State = ConductingReceiptState.Idle;
    }

    private void UpdateState(object sender, RoutedEventArgs e)
    {
        switch (State)
        {
            case ConductingReceiptState.Idle:
            case ConductingReceiptState.Suspended:
                _cancelSource = new CancellationTokenSource();
                State = ConductingReceiptState.Running;

                void Function()
                {
                    try
                    {
                        Receipt[] receipts;
                        using (var ctx = new StorageContext())
                            receipts = [.. ctx.Receipts.Include(r => r.Items)];
                        foreach (var receipt in receipts)
                            ReceiptValidator.Validate(receipt, (int)_handle.DocFormat);
                        while (CurrentIndex < receipts.Length)
                        {
                            _cancelSource.Token.ThrowIfCancellationRequested();
                            _handle.ProcessSession(true);
                            if (ConductingReceipt(receipts[CurrentIndex], _cancelSource.Token))
                                CurrentIndex++;
                            else _handle.CancelReceipt();
                            if (CurrentIndex > 0 && CurrentIndex % Configuration.Current.Fptr.SizeStack == 0)
                                Thread.Sleep(60000 * Configuration.Current.Fptr.StackIntervalMinutes);
                        }
                    }
                    catch (ModelValidationException ex)
                    {
                        logger.Warn(ex, $"{{ModelType}}{{ModelId}}: {ex.Message}", ex.Type, ex.ModelId);
                        MessageBox.Show(ex.Message, "Проверка ФД", 0, MessageBoxImage.Warning);
                    }
                    catch (OperationCanceledException)
                    {
                        MessageBox.Show("Проведение чеков отменено пользователем!");
                    }
                    Finish();
                }
                
                new Thread(Function) { IsBackground = true }.Start();
                break;
            case ConductingReceiptState.Running:
                _cancelSource.Cancel();
                break;
            default: break;
        }
    }

    private void SetIndex(object sender, RoutedEventArgs e) => CurrentIndex = SettingIndex;

    private void SwitchConnection(object sender, RoutedEventArgs e)
    {
        if (State == ConductingReceiptState.Disconnected)
        {
            if (!ConnectKkt())
            {
                _handle.Disconnect();
                return;
            }
            State = ConductingReceiptState.Idle;
        }
        else
        {
            _handle.Disconnect();
            State = ConductingReceiptState.Disconnected;
        }
    }
    
    private void Test(object sender, RoutedEventArgs e)
    {
        const int defaultCount = 2;
        const string text = 
            "Тестовые чеки были проведены!\n" +
            "Отменить тестовые чеки?";
            
        CurrentIndex = 0;
        int count;
        Receipt[] slice = [];
        using (var ctx = new StorageContext()) {
            var receipts = ctx.Receipts.Include(r => r.Items);
            try
            {
                foreach (var receipt in receipts)
                    ReceiptValidator.Validate(receipt, (int)_handle.DocFormat);
            }
            catch(ModelValidationException ex)
            {
                logger.Warn(ex, $"{{ModelType}}{{ModelId}}: {ex.Message}", ex.Type, ex.ModelId);
                MessageBox.Show(ex.Message, "Проверка ФД", 0, MessageBoxImage.Warning);
            }
            count = Math.Min(defaultCount, receipts.Count());
            slice = receipts.ToArray()[..count];
        }

        State = ConductingReceiptState.TestConducting;

        void Function()
        {
            while (CurrentIndex < slice.Length)
            {
                _handle.ProcessSession(true);
                if (ConductingReceipt(slice[CurrentIndex], CancellationToken.None))
                    CurrentIndex++;
                else _handle.CancelReceipt();
            }
            State = ConductingReceiptState.Idle;
            var result = MessageBox.Show(text, "", MessageBoxButton.YesNo);
            if(result == MessageBoxResult.No) return;
            var docNumb = _handle.LastReceiptNumber - (uint)count + 1u;
            State = ConductingReceiptState.TestCancelling;
            for (uint i = 0; i < count; i++)
            {
                var info = _handle.GetReceiptInfo(docNumb + i);
                var receipt = slice[i].Clone();
                receipt.DocSign = FiscalSign.Parse(info.Item1);
                receipt.Created = info.Item2;
                receipt.Type = slice[i].Type switch
                {
                    ReceiptType.Income => ReceiptType.RefundIncome,
                    ReceiptType.RefundIncome => ReceiptType.Income,
                    ReceiptType.Outcome => ReceiptType.RefundOutcome,
                    ReceiptType.RefundOutcome => ReceiptType.Outcome,
                    _ => throw new NotImplementedException()
                };
                _handle.ProcessSession(true);
                if (!ConductingReceipt(receipt))
                {
                    i--;
                    MessageBox.Show(_handle.ErrorDescription, CaptionError, 0, MessageBoxImage.Error);
                    State = ConductingReceiptState.Idle;
                }
                CurrentIndex--;
            }

            State = ConductingReceiptState.Idle;
            MessageBox.Show("Тестовые чеки отменены!");
        }
        new Thread(Function) { IsBackground = true }.Start();
    }
}