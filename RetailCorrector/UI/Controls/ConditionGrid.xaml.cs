using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.UI.Controls;

public partial class ConditionGrid : Grid, INotifyPropertyChanged
{
    private const int MaxCountItems = 10;
    
    public ObservableCollection<ConditionItem> Items { get; } = [];

    public bool CanUserAddItem => Items.Count < MaxCountItems;
    public bool CanUserRemoveItem => SelectedIndex != -1;

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly PropertyChangedEventArgs CanUserAddItemChangedEvent = new(nameof(CanUserAddItem)); 
    private static readonly PropertyChangedEventArgs CanUserRemoveItemChangedEvent = new(nameof(CanUserRemoveItem)); 
    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), 
        typeof(int), typeof(ConditionGrid), new PropertyMetadata(-1, 
            (@object, _) => ((ConditionGrid)@object).PropertyChanged?.Invoke(@object, CanUserRemoveItemChangedEvent))); 

    
    public ConditionGrid()
    {
        InitializeComponent();
        Items.CollectionChanged += (_, _) => PropertyChanged?.Invoke(this, CanUserAddItemChangedEvent);
    }

    private void AddItem(object sender, RoutedEventArgs e)
    {
        if (!CanUserAddItem) return;
        Items.Add(new ConditionItem());
    }

    private void RemoveItem(object sender, RoutedEventArgs e)
    {
        if (!CanUserRemoveItem) return;
        Items.RemoveAt(SelectedIndex);
        SelectedIndex = -1;
    }
}