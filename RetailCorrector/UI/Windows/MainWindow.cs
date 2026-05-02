using System.Globalization;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils.Integrations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors.Core;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace RetailCorrector.UI.Windows;

public partial class MainWindow : Window
{
    public static MainWindow Instance => field ??= new MainWindow();

    private MainWindow()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
        
        Title = "RetailCorrector";
        InputBindings.Add(new KeyBinding(new ActionCommand(ShowPaste), Key.V, ModifierKeys.Control));

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
        var menu = new RetailCorrector.Views.Controls.MainMenu();
        DockPanel.SetDock(menu, Dock.Top);
        dock.Children.Add(menu);
        dock.Children.Add(content);

        Content = dock;
    }

    private static void ShowPaste()
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