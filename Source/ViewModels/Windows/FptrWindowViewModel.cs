using Correctum.Abstractions;
using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Enums;
using Correctum.Enums.ReceiptMetadata;
using Correctum.Exceptions;
using Correctum.Resources;
using Correctum.Resources.Locales;
using Correctum.Utils;
using Correctum.Utils.Integrations;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Correctum.ViewModels.Windows;

internal sealed class FptrWindowViewModel : BindableBase
{
    public string ActNumber { get; set => SetField(ref field, value); }
    public bool ForInternet { get; set => SetField(ref field, value); }
    public int CorrectionType { get; set => SetField(ref field, value); }
    public bool CanSetupReceiptCreating => Stage == FptrJobStage.Idle;
    public int CurrentReceiptIndex { get; private set => SetField(ref field, value); }
    public int TotalReceiptsCount { get; init; }

    public static Geometry SettingsGeometry => Geometry.Parse(SvgPaths.mdiCog);
    public Geometry ConnectionGeometry => Geometry.Parse(driver.IsConnected ? SvgPaths.mdiPowerPlugOff : SvgPaths.mdiPowerPlug);
    public static Geometry TestGeometry => Geometry.Parse(SvgPaths.mdiTestTube);
    public Geometry RunningTaskGeometry => Geometry.Parse(Stage == FptrJobStage.TaskRunning ? SvgPaths.mdiPause : SvgPaths.mdiPlay);
    public static Geometry GoToGeometry => Geometry.Parse(SvgPaths.mdiOpenInNew);

    public ICommand ShowSettingsCommand { get; }
    public ICommand SwitchConnectionCommand { get; }
    public ICommand RunTestReceiptCommand { get; }
    public ICommand SwitchRunningCommand { get; }
    public ICommand GoToReceiptIndexCommand { get; }

    private readonly FptrHandle driver;
    private FptrJobStage Stage
    {
        get; set
        {
            if (SetField(ref field, value))
            {
                OnPropertyChanged(nameof(RunningTaskGeometry));
                OnPropertyChanged(nameof(CanSetupReceiptCreating));
            }
        }
    } = FptrJobStage.Idle;

    private ImmutableArray<(Receipt, ActionMode)> Receipts { get; set; }
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public FptrWindowViewModel()
    {
        driver = new FptrHandle();
        using (var storage = new StorageContext())
            TotalReceiptsCount = storage.MetadataChanges
                .OrderByDescending(m => m.Id)
                .Where(m => m.Property == ReceiptMetadataProperty.ActionMode)
                .AsEnumerable().DistinctBy(m => m.ReceiptId)
                .Count(m => m.Value != (int)ActionMode.Ignore);
        ShowSettingsCommand = new RelayCommand(ShowSettings, CanExecuteShowSettingsCommand);
        SwitchConnectionCommand = new RelayCommand(SwitchConnection, CanExecuteSwitchConnectionCommand);
        RunTestReceiptCommand = new RelayCommand(RunTestReceipts, CanExecuteRunTestReceiptCommand);
        SwitchRunningCommand = new RelayCommand(SwitchRunning, CanExecuteSwitchRunningCommand);
        GoToReceiptIndexCommand = new RelayCommand<int>(SetReceiptIndex, CanExecuteGoToReceiptIndexCommand);
    }

    private bool CanExecuteShowSettingsCommand() => !driver.IsConnected;
    private bool CanExecuteSwitchConnectionCommand() =>
        Stage == FptrJobStage.Idle || Stage == FptrJobStage.TaskPaused;
    private bool CanExecuteRunTestReceiptCommand() =>
        Stage == FptrJobStage.Idle && driver.IsConnected;
    private bool CanExecuteSwitchRunningCommand() =>
        driver.IsConnected && (Stage == FptrJobStage.TaskPaused || Stage == FptrJobStage.TaskRunning);

    private void SetReceiptIndex(int index) => CurrentReceiptIndex = index - 1;
    private bool CanExecuteGoToReceiptIndexCommand(int index) =>
        Stage == FptrJobStage.Idle || Stage == FptrJobStage.TaskPaused;

    private void ShowSettings() =>
        driver.ShowProperties(App.Current.Windows.Cast<Window>().First(w => w.IsActive));

    private void SwitchConnection()
    {
        if (driver.IsConnected)
            driver.Disconnect();
        else ConnectCashbox();
        OnPropertyChanged(nameof(ConnectionGeometry));
    }

    private void ConnectCashbox()
    {
        // Попытка подключения к фискальному регистратору.
        if (!driver.Connect())
        {
            var ex = new FptrException(driver);
            Logger.Error(ex, ErrorMessageLocales.FailedCashboxConnectionMessage);
            var builder = new StringBuilder();
            builder.AppendLine(ErrorMessageLocales.FailedCashboxConnectionMessage);
            builder.AppendFormat(ErrorMessageLocales.FptrErrorDescriptionTemplate, ex.Message);
            MessageBox.Show(builder.ToString(), string.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
            if (driver.IsConnected) driver.Disconnect();
            return;
        }

        // Запуск проверки проблем фискального регистратора.
        var problems = CheckDevice.SearchProblemsInKkt(driver);
        if (problems == AtolKktProblems.None) return;

        // Обработка "списка" проблем ФР
        var (caption, image) = problems.HasErrors ? 
            (NotificationLocales.FoundCashboxCriticalProblemsCaption, MessageBoxImage.Error) : 
            (NotificationLocales.FoundCashboxProblemsCaption, MessageBoxImage.Warning);
        var textBuilder = new StringBuilder();
        textBuilder.AppendLine(NotificationLocales.FoundCashboxProblemsDescriptionBeginning);
        ProblemsLocales
            .Where(pl => problems.HasFlag(pl.PossibleProblem))
            .ForEach(pl => textBuilder.AppendLine($"  - {pl.Locale}"));

        // Сообщение о найденных проблемах ФР.
        MessageBox.Show(textBuilder.ToString(), caption, MessageBoxButton.OK, image);
    }

    private void RunTestReceipts()
    {
        Receipt receipt;
        using (var storage = new StorageContext())
        {
            var receipts = storage.Receipts
                .Include(r => r.Items)
                .ThenInclude(i => i.Changes)
                .Include(r => r.Metadata)
                .Include(r => r.Changes);
            var changed = receipts
                .Where(r => r.Metadata
                    .OrderByDescending(m => m.Id)
                    .FirstOrDefault(m => m.Property == ReceiptMetadataProperty.ActionMode, new() { Value = 0 })
                    .Value != (int)ActionMode.Ignore)
                .AsNoTracking().ToImmutableList();
            receipt = changed[CurrentReceiptIndex];
        }
        var testInfo = new (string FiscalSign, DateTime CreatedAt)[2];

        Stage = FptrJobStage.TrialRunning;
        if(RecordCorrectionReceipt(receipt.CancellationClone()) is { } cancellationInfo)
            testInfo[0] = cancellationInfo;
        else
        {

        }

        if (receipt.Metadata.OrderByDescending(m => m.Id)
            .First(m => m.Property == ReceiptMetadataProperty.ActionMode).Value == (int)ActionMode.Correction)
        {
            if (RecordCorrectionReceipt(ChangesExecutor.ApplyChanges(receipt)) is { } correctionInfo)
                testInfo[1] = correctionInfo;
            else
            {

            }
        }

        Stage = FptrJobStage.TrialChecking;
        var answer = MessageBox.Show(
            """
            Сформированные чеки коррекции передаются в ОФД. 
            Необходима проверка их реквизитов клиентом!

            У клиента есть претензии к реквизитам тестовых чеков?
            """, "Тестовые ЧК сформированы.", MessageBoxButton.YesNo);
        switch (answer)
        {
            case MessageBoxResult.Yes:
                Stage = FptrJobStage.TrialCancelling;
                if(testInfo[1].FiscalSign != string.Empty)
                {
                    var corrReceipt = ChangesExecutor.ApplyChanges(receipt);
                    var cancelCorrReceipt = corrReceipt.CancellationClone();
                    cancelCorrReceipt.Created = testInfo[1].CreatedAt;
                    cancelCorrReceipt.DocSign = FiscalSign.Parse(testInfo[1].FiscalSign);
                    RecordCorrectionReceipt(cancelCorrReceipt);
                }
                receipt.Created = testInfo[0].CreatedAt;
                receipt.DocSign = FiscalSign.Parse(testInfo[0].FiscalSign);
                RecordCorrectionReceipt(receipt.CancellationClone());
                Stage = FptrJobStage.Idle;
                MessageBox.Show("Тестовые чек успешно аннулированы!");
                break;
            case MessageBoxResult.No:
                CurrentReceiptIndex++;
                using (var storage = new StorageContext())
                {
                    var receipts = storage.Receipts
                        .Include(r => r.Items)
                        .ThenInclude(i => i.Changes)
                        .Include(r => r.Metadata)
                        .Include(r => r.Changes);
                    var changed = receipts
                        .Where(r => r.Metadata
                            .OrderByDescending(m => m.Id)
                            .FirstOrDefault(m => m.Property == ReceiptMetadataProperty.ActionMode, new() { Value = 0 })
                            .Value != (int)ActionMode.Ignore)
                        .AsNoTracking().AsEnumerable();
                    Receipts = [..changed
                        .Select(r => (r, (ActionMode)r.Metadata
                            .OrderByDescending(m => m.Id)
                            .First(m => m.Property == ReceiptMetadataProperty.ActionMode).Value))];
                }
                Stage = FptrJobStage.TaskPaused;
                break;
        }
    }

    private (string FiscalSign, DateTime CreatedAt)? RecordCorrectionReceipt(Receipt receipt)
    {
        if (!driver.OpenReceipt(receipt, new())) return InternalCancel();
        if (!receipt.Items.All(driver.AddPosition)) return InternalCancel();
        if (!driver.SetTotal(receipt.TotalSum!.Value)) return InternalCancel();
        if (!driver.Payment(receipt)) return InternalCancel();
        if (!driver.CloseReceipt()) return InternalCancel();
        return driver.GetReceiptInfo(driver.LastReceiptNumber);

        (string, DateTime)? InternalCancel()
        {
            driver.CancelReceipt();
            return null;
        }        
    }

    private async void SwitchRunning()
    {
        if(Stage == FptrJobStage.TaskPaused)
        {
            Stage = FptrJobStage.TaskRunning;
            ThreadPool.QueueUserWorkItem(_ => RecordCorrectionReceipts());
        }
        else Stage = FptrJobStage.TaskPausing;
    }

    private void RecordCorrectionReceipts()
    {
        while (Stage == FptrJobStage.TaskRunning)
        {
            var receipt = Receipts[CurrentReceiptIndex].Item1;
            RecordCorrectionReceipt(receipt.CancellationClone());
            if (Receipts[CurrentReceiptIndex].Item2 == ActionMode.Correction)
                RecordCorrectionReceipt(ChangesExecutor.ApplyChanges(receipt));
            CurrentReceiptIndex++;
        }
        if (Stage == FptrJobStage.TaskPausing)
            Stage = FptrJobStage.TaskPaused;
    }

    public void OnClosing()
    {
        driver.Dispose();
    }

    private enum FptrJobStage
    {
        /// <summary>
        /// Ожидание запуска формирования чеков.
        /// Доступные действия: запуск формирования тестовых чеков.
        /// </summary>
        Idle,
        /// <summary>
        /// Формирование тестовых чеков.
        /// </summary>
        TrialRunning,
        /// <summary>
        /// Проверка тестовых чеков.
        /// Доступные действия: аннулирование тестовых чеков, запуск массового формирования чеков.
        /// </summary>
        TrialChecking,
        /// <summary>
        /// Аннулирование тестовых чеков.
        /// По завершению - закрывает окно.
        /// </summary>
        TrialCancelling,
        /// <summary>
        /// Запуск массового формирования чеков.
        /// </summary>
        TaskRunning,
        /// <summary>
        /// Массовое формирование чеков приостановливается.
        /// </summary>
        TaskPausing,
        /// <summary>
        /// Массовое формирование чеков приостановлено.
        /// </summary>
        TaskPaused,
        /// <summary>
        /// Массовое формирование чеков завершено, проверка чеков.
        /// Повторный запуск запрещен.
        /// </summary>
        TaskFinished
    }

    private static readonly ImmutableArray<(AtolKktProblems PossibleProblem, string Locale)> ProblemsLocales = [
        (AtolKktProblems.AdditionalPrintExpired,    ErrorMessageLocales.AtolProblemAdditionalPrintExpiredOption),
        (AtolKktProblems.AdditionalPrintNoFound,    ErrorMessageLocales.AtolProblemAdditionalPrintNotFoundOption),
        (AtolKktProblems.ConfigurationOutdated,     ErrorMessageLocales.AtolProblemConfigurationUnitOutdatedOption),
        (AtolKktProblems.ModelKktUnknown,           ErrorMessageLocales.CashboxProblemUnknownModelOption),
        (AtolKktProblems.CheckKktUnknown,           ErrorMessageLocales.CashboxProblemFailedCheckingOption),
        (AtolKktProblems.CheckKktError,             ErrorMessageLocales.CashboxProblemErrorCheckingOption),
        (AtolKktProblems.CheckKktNeedRegister,      ErrorMessageLocales.CashboxProblemRegistrationProcessingOption),
        (AtolKktProblems.CheckKktNoCreated,         ErrorMessageLocales.CashboxProblemNotFoundOption),
        (AtolKktProblems.ModelFnUnknown,            ErrorMessageLocales.FiscalStorageProblemUnknownModelOption),
        (AtolKktProblems.CheckFnUnknown,            ErrorMessageLocales.FiscalStorageProblemFailedCheckingOption),
        (AtolKktProblems.CheckFnError,              ErrorMessageLocales.FiscalStorageProblemErrorCheckingOption),
        (AtolKktProblems.CheckFnNeedRegister,       ErrorMessageLocales.FiscalStorageProblemRegistrationProcessingOption),
        (AtolKktProblems.CheckFnNoCreated,          ErrorMessageLocales.FiscalStorageProblemNotFoundOption),
        (AtolKktProblems.CheckFnArchived,           ErrorMessageLocales.FiscalStorageProblemArchivedOption)
    ];
}
