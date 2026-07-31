using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Correctum.Views.Controls;

public sealed class DataGridReceiptInfoColumn : DataGridBoundColumn
{
    public static IValueConverter BackgroundConverter => 
        field ?? (IValueConverter)App.Current.FindResource("nullIndicator");

    public HorizontalAlignment HorizontalTextAlignment
    {
        get => (HorizontalAlignment)GetValue(HorizontalTextAlignmentProperty);
        set => SetValue(HorizontalTextAlignmentProperty, value);
    }

    public readonly static DependencyProperty HorizontalTextAlignmentProperty =
        DependencyProperty.Register(nameof(HorizontalTextAlignment), typeof(HorizontalAlignment),
            typeof(DataGridReceiptInfoColumn), new PropertyMetadata(HorizontalAlignment.Center));

    public DataGridReceiptInfoColumn()
    {
        HeaderStyle = new Style();
        HeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.ForegroundProperty, Brushes.Black));
        HeaderStyle.Setters.Add(new Setter(DataGridColumnHeader.HorizontalContentAlignmentProperty, HorizontalTextAlignment));
    }

    protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        => GenerateElementInternal();

    protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        => GenerateElementInternal();

    private Grid GenerateElementInternal()
    {
        var panel = new Grid { Margin = new(5,0,5,0) };
        panel.SetBinding(Grid.BackgroundProperty, new Binding(((Binding)Binding).Path.Path)
        {
            Converter = BackgroundConverter
        });

        var textBlock = new TextBlock
        {
            Foreground = Brushes.Black,
            HorizontalAlignment = HorizontalTextAlignment
        };
        textBlock.SetBinding(TextBlock.TextProperty, Binding);

        panel.Children.Add(textBlock);

        return panel;
    }
}
