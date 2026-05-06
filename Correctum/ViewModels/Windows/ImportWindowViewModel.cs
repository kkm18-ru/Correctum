using Correctum.Core.Models;
using Correctum.Importers;
using Correctum.UI.Controls;
using Correctum.Utils;
using System.Windows;
using System.Windows.Input;

namespace Correctum.ViewModels.Windows;

public sealed class ImportWindowViewModel : BaseViewModel
{
    public string ImportName { get; }
    public ICommand StartCommand { get; }
    public ICommand CancelCommand { get; }

    public int CurrentProgress { get; set; }
    public int MaximumProgress { get; set; }

    public event Action? Imported;

    private CancellationTokenSource cts = null!;
    private readonly static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly BoundedProgress<uint> progress;
    private readonly IImporter importer;

    internal ImportWindowViewModel(string importName, IImporter importer)
    {
        ImportName = importName;
        StartCommand = new RelayCommand(Start, () => cts?.IsCancellationRequested ?? true);
        CancelCommand = new RelayCommand(Cancel, () => !cts?.IsCancellationRequested ?? false);

        progress = new BoundedProgress<uint>();
        this.importer = importer;
    }

#if DEBUG
    /// <summary>
    /// Инициализация тестового экземпляра <see cref="ImportWindowViewModel"/>
    /// </summary>
    public ImportWindowViewModel() : this("Mock", new Mock())
    {
        CurrentProgress = 10;
        MaximumProgress = 100;
    }
#endif

    private async void Start()
    {
        var buffer = new List<Receipt>();
        cts = new CancellationTokenSource();
        try
        {
            await foreach (var receipt in importer.Import(progress, cts.Token))
                buffer.Add(receipt);
            cts.Dispose();
            buffer.ForEach(Workspace.Instance.Receipts.Add);
            Imported?.Invoke();
        }
        catch (OperationCanceledException e)
        {
            Logger.Warn(e, "Импортирование чеков была отменена пользователем");
            MessageBox.Show("Выгрузка чеков была отменена пользователем", "", 0, MessageBoxImage.Hand);
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, "Неизвестная ошибка");
            MessageBox.Show(ex.Message, "Ошибка выгрузки чеков", 0, MessageBoxImage.Error);
            Cancel();
        }
    }

    private void Cancel() => cts?.Cancel();

    public void OnClosing() => importer.OnClosing();
}
