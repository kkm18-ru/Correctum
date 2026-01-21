using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using RetailCorrector.Enums.Condition;
using RetailCorrector.Utils;
using Expression = System.Linq.Expressions.Expression;

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
    
    public ConditionSubjectType SubjectType
    {
        get => (ConditionSubjectType)GetValue(SubjectTypeProperty);
        set => SetValue(SubjectTypeProperty, value);
    }

    public static readonly DependencyProperty SubjectTypeProperty = DependencyProperty.Register(
        nameof(SubjectType), typeof(ConditionSubjectType), typeof(ConditionGrid));
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly PropertyChangedEventArgs CanUserAddItemChangedEvent = new(nameof(CanUserAddItem)); 
    private static readonly PropertyChangedEventArgs CanUserRemoveItemChangedEvent = new(nameof(CanUserRemoveItem)); 
    public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register(nameof(SelectedIndex), 
        typeof(int), typeof(ConditionGrid), new PropertyMetadata(-1, 
            (@object, _) => ((ConditionGrid)@object).PropertyChanged?.Invoke(@object, CanUserRemoveItemChangedEvent))); 

    
    public ConditionGrid()
    {
        InitializeComponent();
        Items.CollectionChanged += OnItemsChanged;
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, CanUserAddItemChangedEvent);
        for (var i = 0; i < Items.Count; i++)
            Items[i].Id = i + 1;
    } 

    private void AddItem(object sender, RoutedEventArgs e)
    {
        if (!CanUserAddItem) return;
        Items.Add(new ConditionItem { SubjectType = SubjectType });
    }

    private void RemoveItem(object sender, RoutedEventArgs e)
    {
        if (!CanUserRemoveItem) return;
        Items.RemoveAt(SelectedIndex);
        SelectedIndex = -1;
    }

    public Func<IUnionReceiptPosition, bool>? Compile()
    {
        if (Items.Count == 0) return _ => true;
        var input = Expression.Parameter(typeof(IUnionReceiptPosition));
        var firstExpression = Items[0].ToExpression(input);
        if (firstExpression is null)
        {
            MessageBox.Show("Найдено некорректное условие №1", "", 0, MessageBoxImage.Error);
            return null;
        }
        var expression = firstExpression;

        for (var i = 1; i < Items.Count; i++)
        {
            var tempExpression = Items[i].ToExpression(input);
            if (tempExpression is null)
            {
                MessageBox.Show($"Найдено некорректное условие №{i + 1}", "", 0, MessageBoxImage.Error);
                return null;
            }
            expression = Expression.And(expression, tempExpression);
        }
        var lambda = Expression.Lambda<Func<IUnionReceiptPosition, bool>>(expression, input); 
        return lambda.Compile();
    }
}