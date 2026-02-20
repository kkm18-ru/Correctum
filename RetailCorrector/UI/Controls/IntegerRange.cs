using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;

namespace RetailCorrector.UI.Controls;

public class IntegerRange : WrapPanel
{
    public int Minimum
    {
        get => (int)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }
    
    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty MinimumProperty =
        DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(IntegerRange), new PropertyMetadata(1));
    
    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(IntegerRange), new PropertyMetadata(1000));

    protected override void OnInitialized(EventArgs e)
    {
        var widthBinding = new Binding(nameof(ActualWidth))
        {
            Mode = BindingMode.OneWay, 
            Converter = new RangeWidthConverter(),
            Source = this
        }; 
        
        var min = new IntegerUpDown { Minimum = 1 };
        min.SetBinding(IntegerUpDown.WidthProperty, widthBinding);
        min.SetBinding(IntegerUpDown.ValueProperty, nameof(Minimum));
        min.SetBinding(IntegerUpDown.MaximumProperty, nameof(Maximum));
        Children.Add(min);

        Children.Add(new Separator { Width = 5, Height = 0 });
        
        var max = new IntegerUpDown { Maximum = 5000 };
        max.SetBinding(IntegerUpDown.WidthProperty, widthBinding);
        max.SetBinding(IntegerUpDown.MinimumProperty, nameof(Minimum));
        max.SetBinding(IntegerUpDown.ValueProperty, nameof(Maximum));
        Children.Add(max);
        
        base.OnInitialized(e);
    }
}

file sealed class RangeWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value is not double v || v <= 5 ? 0 : (v - 5) / 2;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 0;
}