using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace RetailCorrector.UI.Controls;

[ContentProperty(nameof(Elements))]
public abstract class FormGrid : Grid, INotifyPropertyChanged
{
    public static string FormName { get; }
    public Collection<string> Labels { get; } = [];
    public Collection<UIElement> Elements { get; } = [];
    
    public event PropertyChangedEventHandler? PropertyChanged;

    protected override void OnInitialized(EventArgs e)
    {
        ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5) });
        ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var maxIndex = Math.Max(Labels.Count, Elements.Count) - 1;
        for (var i = 0; i <= maxIndex; i++)
        {
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            if (i < maxIndex) RowDefinitions.Add(new RowDefinition { Height = new GridLength(5) });
            if (i < Labels.Count && !string.IsNullOrWhiteSpace(Labels[i]))
            {
                var label = new TextBlock { Text = Labels[i], VerticalAlignment = VerticalAlignment.Center };
                SetRow(label, i * 2);
                Children.Add(label);
            }
            if (i < Elements.Count)
            {
                SetRow(Elements[i], i * 2);
                SetColumn(Elements[i], 2);
                Children.Add(Elements[i]);
            }
        }
        
        base.OnInitialized(e);
    }

    public virtual void OnClosing(CancelEventArgs e){}
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}