using RetailCorrector.Attributes;
using RetailCorrector.UI.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace RetailCorrector.UI.Windows;

[Singleton]
public partial class MainWindow : Window
{
    private MainWindow()
    {
        Title = "RetailCorrector";
        (Width, Height) = (1000, 500);

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

        Content = new DockingManager
        {
            AllowMixedOrientation = true,
            DocumentHeaderTemplate = headerTemplate,
            Layout = new() { RootPanel = panelLayout }
        };
    }
}