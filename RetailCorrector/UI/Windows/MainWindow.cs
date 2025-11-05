using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace RetailCorrector.UI.Windows;

public partial class MainWindow : Window
{
    private static MainWindow _instance = null!;
    public static MainWindow Instance => _instance ??= new MainWindow();

    private MainWindow()
    {
        Title = "RetailCorrector";
        WindowStyle = WindowStyle.None;
        ResizeMode = ResizeMode.NoResize;
        InputBindings.Add(new KeyBinding(new RelayCommand(ShowPaste), Key.V, ModifierKeys.Control));
        var area = SystemParameters.WorkArea;
        (Width, Height, Top, Left) = (area.Width, area.Height, area.Top, area.Left);

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
        DockPanel.SetDock(TitleBar.Instance, Dock.Top);
        var content = new DockingManager
        {
            AllowMixedOrientation = true,
            DocumentHeaderTemplate = headerTemplate,
            Layout = new() { RootPanel = panelLayout }
        };
        dock.Children.Add(TitleBar.Instance);
        dock.Children.Add(content);

        Content = dock;
    }

    private static void ShowPaste(object? obj)
    {
        if (!Clipboard.ContainsText()) return;
        new DeserializeWindow(Clipboard.GetText()).Show();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Configuration.Instance.Save();
    }
}