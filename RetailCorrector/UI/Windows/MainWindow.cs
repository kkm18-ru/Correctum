using System.Globalization;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using MessageDialog;
using Microsoft.Win32;

namespace RetailCorrector.UI.Windows;

public partial class MainWindow : Window
{
    public static MainWindow Instance => field ??= new MainWindow();

    private MainWindow()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
                
        Title = "RetailCorrector";
        InputBindings.Add(new KeyBinding(new RelayCommand(ShowPaste), Key.V, ModifierKeys.Control));

        var textHeader = new FrameworkElementFactory(typeof(TextBlock));
        textHeader.SetBinding(TextBlock.TextProperty, new Binding(nameof(LayoutDocument.Title)));
        var panelHeader = new FrameworkElementFactory(typeof(StackPanel));
        panelHeader.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
        panelHeader.AppendChild(textHeader);
        var headerTemplate = new DataTemplate { VisualTree = panelHeader };

        var panelLayout = new LayoutPanel { Orientation = Orientation.Horizontal };
        var pane = new LayoutDocumentPane();
        pane.Children.Add(new LayoutDocument 
        { 
            ContentId = "receipts", 
            Title = "Чеки", 
            CanClose = false, 
            Content = ReceiptPanel.Instance
        });
        pane.Children.Add(new LayoutDocument 
        { 
            ContentId = "positions", 
            Title = "Позиции", 
            CanClose = false, 
            Content = PositionGrid.Instance
        });
        var group = new LayoutDocumentPaneGroup(pane);
        panelLayout.Children.Add(group);

        var dock = new DockPanel();
        var content = new DockingManager
        {
            AllowMixedOrientation = true,
            DocumentHeaderTemplate = headerTemplate,
            Layout = new() { RootPanel = panelLayout }
        };
        var menu = new MainMenu();
        DockPanel.SetDock(menu, Dock.Top);
        dock.Children.Add(menu);
        dock.Children.Add(content);

        Content = dock;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        if (Storage.Instance == null!)
        {
            var createBase = () =>
            {
                var dialog = new SaveFileDialog
                {
                    Filter = "База данных Correctum|*.cdb",
                    AddExtension = true,
                };
                if (!dialog.ShowDialog() ?? false)
                {
                    MessageBox.Show("Путь к базе данных не выбран. Приложение закрывается.");
                    Close();
                }
                Storage.Create(dialog.FileName);
            };
            var loadBase = () =>
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "База данных Correctum|*.cdb",
                };
                if (!dialog.ShowDialog() ?? false)
                {
                    MessageBox.Show("Путь к базе данных не выбран. Приложение закрывается.");
                    Close();
                }
                Storage.Load(dialog.FileName);
            };
            var options = new MessageDialogOptions
            {
                Title = "Хранилище не проинициализировано!",
                Message = "Для инициализации хранилища необходимо открыть существующую базу Correctum или создать новую.",
                Buttons = [
                    new MessageDialogButtonOptions("Создать", createBase),
                    new MessageDialogButtonOptions("Загрузить", loadBase),
                    new MessageDialogButtonOptions("Закрыть", Close, MessageDialogButtonType.Cancel)
                    ]
            };
            new MessageDialog.MessageDialog(this, options).ShowDialog();
        }
        base.OnContentRendered(e);
    }

    private static void ShowPaste(object? obj)
    {
        if (!Clipboard.ContainsText()) return;
        new ParserWindow(Clipboard.GetText()).Show();
    }

    protected override void OnClosed(EventArgs e)
    {
        FptrHandle.FreeLibrary();
        base.OnClosed(e);
    }
}