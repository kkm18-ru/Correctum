using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Controls;

namespace RetailCorrector.UI.Controls;

public partial class ConditionItem : ListBoxItem, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public ConditionItem()
    {
        InitializeComponent();
    }

    internal Expression<Predicate<object>> ToExpression()
    {
        throw new NotImplementedException();
    }
}
