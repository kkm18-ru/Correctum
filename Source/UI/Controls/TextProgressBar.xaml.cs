using System.Windows;
using System.Windows.Controls;

namespace Correctum.UI.Controls;

public sealed partial class TextProgressBar : Grid
{
    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), 
        typeof(TextProgressBar), new PropertyMetadata(0));

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), 
        typeof(TextProgressBar), new PropertyMetadata(0));
    
    public TextProgressBar()
    {
        InitializeComponent();
    }
}