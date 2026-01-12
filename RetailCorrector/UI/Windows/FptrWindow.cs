using RetailCorrector.Data;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RetailCorrector.UI.Windows;

internal sealed class FptrWindow : Types.DialogWindow
{
    private readonly FptrHandle handle;
    private readonly object data;

    private CancellationTokenSource? cts;

    public FptrWindow()
    {
        DataContext = data = new FptrWindowDataContext();
        SizeToContent = SizeToContent.WidthAndHeight;
        handle = new FptrHandle();
        var grid = new Grid
        {
            Margin = new Thickness(5)
        };


        grid.AddColumn(GridUnitType.Pixel, 100);
        grid.AddColumn(GridUnitType.Pixel, 5);
        grid.AddColumn(GridUnitType.Pixel, 150);

        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);
        grid.AddRow(GridUnitType.Pixel, 5);
        grid.AddRow(GridUnitType.Auto);

        grid.AddChild(new Button()
            .SetEnabledBinding()
            .SetClickAction((_, _) => handle.ShowProperties(this))
            .SetContent("Настройка драйвера"), dx: 3);

        grid.AddChild(new CheckBox()
            .SetContent("Печатать ли чек?")
            .SetCheckedBinding(nameof(FptrWindowDataContext.Print))
            .SetEnabledBinding(), y: 2, dx: 3);

        grid.AddChild(new TextBlock()
            .SetText("Вид коррекции:")
            .SetCenter(), y: 4);
        grid.AddChild(new ComboBox()
            .SetItems("Самостоятельная", "По предписанию")
            .SetIndexBinding(nameof(FptrWindowDataContext.CorrectionType))
            .SetEnabledBinding(), 2, 4);

        grid.AddChild(new TextBlock()
            .SetText("Номер акта:")
            .SetToolTip(Locale.ActTip)
            .SetCenter(), y: 6);
        grid.AddChild(new TextBox()
            .SetTextBinding(nameof(FptrWindowDataContext.Act))
            .SetEnabledBinding(), 2, 6);

        grid.AddChild(new TextBlock()
            .SetText("ФИО оператора:")
            .SetCenter(), y: 8);
        grid.AddChild(new TextBox()
            .SetTextBinding(nameof(FptrWindowDataContext.OperatorName))
            .SetEnabledBinding(), 2, 8);

        grid.AddChild(new TextBlock()
            .SetText("ИНН оператора:")
            .SetCenter(), y: 10);
        grid.AddChild(new TextBox()
            .SetTextBinding(nameof(FptrWindowDataContext.OperatorINN))
            .SetEnabledBinding(), 2, 10);

        grid.AddChild(new CheckBox()
            .SetContent("Применить рассчет для сети \"Интернет\"")
            .SetCheckedBinding(nameof(FptrWindowDataContext.IsInternet))
            .SetEnabledBinding(), y: 12, dx: 3);

        grid.AddChild(new TextBlock()
            .SetText("Адрес сайта:")
            .SetCenter(), y: 14);
        grid.AddChild(new TextBox()
            .SetTextBinding(nameof(FptrWindowDataContext.Address))
            .SetEnabledBinding(), 2, 14);

        grid.AddChild(CreateProgress(), 0, 16, 3);
        grid.AddChild(new TextBlock()
            .SetTextBinding(Bindings.ProgressText)
            .SetCenter(), 0, 16, 3);

        grid.AddChild(new Button()
            .SetEnabledBinding()
            .SetClickAction(Start)
            .SetContent("Запуск"), y: 18);
        grid.AddChild(new Button()
            .SetEnabledBinding(false)
            .SetContent("Отмена")
            .SetClickAction(Cancel), 2, 18);

        Content = grid;
    }

    private static ProgressBar CreateProgress()
    {
        var _ = new ProgressBar { Maximum = Math.Max(Workspace.Instance.Receipts.Count, 1) };
        _.SetBinding(ProgressBar.ValueProperty, nameof(FptrWindowDataContext.CurrentProgress));
        return _;
    }

    private void Start(object s, RoutedEventArgs e)
    {
        ((FptrWindowDataContext)DataContext).SetStatus(true);
        cts = new CancellationTokenSource();
        new Thread(() => ProcessingReceipts(cts.Token)) { IsBackground = true }.Start();
    }

    private bool CanElectronicallyReceipt()
    {
        const uint printLicenseNumber = 17;
        const string caption = "Проведение электронных чеков невозможна";
        if (handle.ConfigurationVersion < new Version(5, 12))
        {
            MessageBox.Show("Версия конфигурации сильно устарела.", 
                caption, 0, MessageBoxImage.Stop);
            return false;
        }
        var license = handle.GetLicenseInfo(printLicenseNumber);
        if (!(license?.Entered ?? false))
        {
            MessageBox.Show("Лицензия \"Доп. функционал Печать\" не введена!", 
                caption, 0, MessageBoxImage.Stop);
            return false;
        }
        var today = DateTime.Today;
        if ((license?.ValidUntil ?? DateTime.MinValue) > today) return true;
        MessageBox.Show("Срок действия лицензии \"Доп. функционал Печать\" истек!", 
            caption, 0, MessageBoxImage.Stop);
        return false;
    }

    private void ProcessingReceipts(CancellationToken token)
    {
        var context = (FptrWindowDataContext)data;
        try
        {
            handle.Connect();
            if (!CanElectronicallyReceipt()) return;
            if (string.IsNullOrWhiteSpace(context.OperatorName) &&
                !handle.LoginOperator(context.OperatorName, context.OperatorINN))
            {
                MessageBox.Show("Не удалось зарегистрировать кассира", $"Произошла ошибка (Код: {handle.ErrorCode})",
                    MessageBoxButton.OK, MessageBoxImage.Hand);
                Cancel(null, null!);
                token.ThrowIfCancellationRequested();
            }
            var @ref = Workspace.Instance.Receipts;
            if (MissingUnitFor120(@ref))
            {
                MessageBox.Show("На ФФД 1.2 единицы измерения обязаны присутствовать!");
                return;
            }

            context.CurrentProgress = 0;
            while (context.CurrentProgress < @ref.Count)
            {
                token.ThrowIfCancellationRequested();
                if (handle.ProcessSession(!((FptrWindowDataContext)data).Print) &&
                    ProcessingReceipt(@ref[context.CurrentProgress], token))
                    context.CurrentProgress++;
                else handle.CancelReceipt();
                if (context.CurrentProgress > 0 && context.CurrentProgress % Configuration.Instance.Fptr.CountReceiptsBeforeDelay == 0)
                    Thread.Sleep(60000 * Configuration.Instance.Fptr.Delay);
            }
            Cancel(null, null!);
        }
        catch (OperationCanceledException)
        {
            handle.CancelReceipt();
            handle.Disconnect();
        }
    }

    private void Cancel(object? s, RoutedEventArgs e)
    {
        ((FptrWindowDataContext)data).SetStatus(false);
        cts?.Cancel();
        cts?.Dispose();
    }

    private bool ProcessingReceipt(Receipt receipt, CancellationToken token)
    {
        var context = (FptrWindowDataContext)data;
        token.ThrowIfCancellationRequested();
        OpenReceiptArgs openArgs = new();
        Dispatcher.Invoke(() =>
        {
            openArgs.Act = string.IsNullOrWhiteSpace(context.Act) ? " " : context.Act;
            openArgs.Print = context.Print;
            openArgs.CorrType = context.CorrectionType;
            openArgs.IsInternetPayment = context.IsInternet;
            openArgs.HttpAddress = context.Address;
        });
        return handle.OpenReceipt(receipt, openArgs) &&
            receipt.Items.All(handle.AddPosition) &&
            handle.SetTotal(receipt.TotalSum / 100.0) &&
            handle.Payment(receipt.Payment) &&
            handle.CloseReceipt();
    }

    private bool MissingUnitFor120(IEnumerable<Receipt> @ref) =>
        handle.DocFormat == 120 && @ref.SelectMany(r => r.Items).Any(p => p.MeasureUnit is null);

    protected override void OnClosed(EventArgs e)
    {
        if (!handle.IsInvalid)
        {
            handle.CancelReceipt();
            handle.Disconnect();
            handle.Dispose();
        }
    }
}

file sealed class FptrWindowDataContext : INotifyPropertyChanged
{
    public string Act
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, ActKey);
        }
    } = "";
    public string Address
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, AddressKey);
        }
    } = "";
    public string OperatorName
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, OperatorNameKey);
        }
    } = "";
    public string OperatorINN
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, OperatorINNKey);
        }
    } = "";
    public int CurrentProgress
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, CurrentProgressKey);
        }
    }
    public bool IsInternet
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, IsInternetKey);
        }
    }
    public int CorrectionType
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, CorrectionTypeKey);
        }
    }
    public bool Print
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, PrintKey);
        }
    } = true;
    public bool Processing => processing;
    public bool Idle => !processing;

    public void SetStatus(bool processing)
    {
        this.processing = processing;
        PropertyChanged?.Invoke(this, IdleKey);
        PropertyChanged?.Invoke(this, ProcessingKey);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly PropertyChangedEventArgs ActKey = new(nameof(Act));
    private readonly PropertyChangedEventArgs AddressKey = new(nameof(Address));
    private readonly PropertyChangedEventArgs OperatorNameKey = new(nameof(OperatorName));
    private readonly PropertyChangedEventArgs OperatorINNKey = new(nameof(OperatorINN));
    private readonly PropertyChangedEventArgs CurrentProgressKey = new(nameof(CurrentProgress));
    private readonly PropertyChangedEventArgs IsInternetKey = new(nameof(IsInternet));
    private readonly PropertyChangedEventArgs PrintKey = new(nameof(Print));
    private readonly PropertyChangedEventArgs CorrectionTypeKey = new(nameof(CorrectionType));
    private readonly PropertyChangedEventArgs ProcessingKey = new(nameof(Processing));
    private readonly PropertyChangedEventArgs IdleKey = new(nameof(Idle));

    private bool processing = false;
}

file static class FptrWindowHelper
{

    public static T SetEnabledBinding<T>(this T element, bool idle = true)
        where T : FrameworkElement
    {
        element.SetBinding(UIElement.IsEnabledProperty,
            idle ? Bindings.IdleEnabled : Bindings.ProcessingEnabled);
        return element;
    }

    public static T SetToolTip<T>(this T element, string text)
        where T : FrameworkElement
    {
        element.ToolTip = new ToolTip { Content = text };
        return element;
    }

    public static TextBox SetTextBinding(this TextBox element, string path)
    {
        element.SetBinding(TextBox.TextProperty, path);
        return element;
    }

    public static TextBlock SetTextBinding(this TextBlock element, BindingBase binding)
    {
        element.SetBinding(TextBlock.TextProperty, binding);
        return element;
    }

    public static TextBlock SetText(this TextBlock element, string value)
    {
        element.Text = value;
        return element;
    }

    public static CheckBox SetCheckedBinding(this CheckBox element, string path)
    {
        element.SetBinding(CheckBox.IsCheckedProperty, path);
        return element;
    }

    public static ComboBox SetIndexBinding(this ComboBox element, string path)
    {
        element.SetBinding(ComboBox.SelectedIndexProperty, path);
        return element;
    }

    public static T SetContent<T>(this T element, string value)
        where T : ContentControl
    {
        element.Content = value;
        return element;
    }

    public static ComboBox SetItems(this ComboBox element, params IEnumerable<string> items)
    {
        element.ItemsSource = items;
        return element;
    }

    public static Button SetClickAction(this Button element, RoutedEventHandler handler)
    {
        element.Click += handler;
        return element;
    }

    public static T SetCenter<T>(this T element)
        where T : FrameworkElement
    {
        element.HorizontalAlignment = HorizontalAlignment.Center;
        element.VerticalAlignment = VerticalAlignment.Center;
        return element;
    }
}

file static class Locale
{
    public const string ActTip =
        """
        Данный параметр нужно использовать только в двух случаях:
        1) Коррекция выполняется по требованию ФНС
        2) Коррекция выполняется на кассе с ФФД 1.05, так как чек ранее не был сформирован 
        """;
}

file static class Bindings
{
    static Bindings()
    {
        ProgressText = new MultiBinding();
        ProgressText.Bindings.Add(new Binding(nameof(FptrWindowDataContext.CurrentProgress)));
        ProgressText.Bindings.Add(new Binding(nameof(Workspace.Instance.Receipts.Count))
        {
            Source = Workspace.Instance.Receipts
        });
        ProgressText.StringFormat = "{0} / {1}";
    }

    public readonly static MultiBinding ProgressText;

    public readonly static Binding IdleEnabled =
        new(nameof(FptrWindowDataContext.Idle))
        { Mode = BindingMode.OneWay };

    public readonly static Binding ProcessingEnabled =
        new(nameof(FptrWindowDataContext.Processing))
        { Mode = BindingMode.OneWay };
}