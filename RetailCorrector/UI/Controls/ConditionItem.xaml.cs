using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.Windows.Data;
using RetailCorrector.Data;
using RetailCorrector.Enums;
using Xceed.Wpf.Toolkit;
using RetailCorrector.Enums.Condition;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Expressions;

namespace RetailCorrector.UI.Controls;

public partial class ConditionItem : ListBoxItem, INotifyPropertyChanged
{
    public int Id
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, IdChangedArgs);
        }
    }
    
    public required ConditionSubjectType SubjectType
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, SubjectTypeChangedArgs);
            Subject = ConditionSubject.None;
            UpdateLocaleSubjects();
        }
    }

    public ConditionSubject Subject
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, SubjectChangedArgs);
            Prop = ConditionProp.None;
            UpdateLocaleProps();
        }
    } = ConditionSubject.None;
    
    public ConditionProp Prop
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, PropChangedArgs);
            Operator = ConditionOperator.None;
            UpdateLocaleOperators();
            Value = null;
            UpdateValueControl();
        }
    } = ConditionProp.None;
    
    public ConditionOperator Operator
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, OperatorChangedArgs);
        }
    } = ConditionOperator.None;
    
    private ConditionValueType ValueType => Prop switch
    {
        ConditionProp.DeviceId or ConditionProp.DriveId or ConditionProp.Buyer or ConditionProp.Name =>
            ConditionValueType.Text,
        ConditionProp.Session => ConditionValueType.Integer,
        ConditionProp.Created => ConditionValueType.DateTime,
        ConditionProp.Taxation => ConditionValueType.Flag,
        ConditionProp.ReceiptType or ConditionProp.PaymentMethod or ConditionProp.Unit
            or ConditionProp.PositionType or ConditionProp.TaxRate => ConditionValueType.Enum,
        ConditionProp.IsReadOnly => ConditionValueType.Boolean,
        ConditionProp.CashSum or ConditionProp.EcashSum or ConditionProp.PreSum or ConditionProp.PostSum
            or ConditionProp.OtherSum or ConditionProp.TotalSum or ConditionProp.Price or ConditionProp.Quantity
            or ConditionProp.Cost => ConditionValueType.Float,
        _ => ConditionValueType.None
    };
    
    public object? Value
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, ValueChangedArgs);
        }
    }
    
    public System.Windows.FrameworkElement? ValueControl
    {
        get;
        set
        {
            if(field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, ValueControlChangedArgs);
        }
    }

    public ObservableCollection<EnumLocale<ConditionSubject>> LocaleSubjects { get; } = [];
    public ObservableCollection<EnumLocale<ConditionProp>> LocaleProps { get; } = [];
    public ObservableCollection<EnumLocale<ConditionOperator>> LocaleOperators { get; } = [];
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly PropertyChangedEventArgs IdChangedArgs = new(nameof(Id));
    private static readonly PropertyChangedEventArgs SubjectTypeChangedArgs = new(nameof(SubjectType));
    private static readonly PropertyChangedEventArgs SubjectChangedArgs = new(nameof(Subject));
    private static readonly PropertyChangedEventArgs PropChangedArgs = new(nameof(Prop));
    private static readonly PropertyChangedEventArgs OperatorChangedArgs = new(nameof(Operator));
    private static readonly PropertyChangedEventArgs ValueChangedArgs = new(nameof(Value));
    private static readonly PropertyChangedEventArgs ValueControlChangedArgs = new(nameof(ValueControl));
    
    public ConditionItem()
    {
        InitializeComponent();
        UpdateLocaleSubjects();
    }

    private void UpdateLocaleSubjects()
    {
        LocaleSubjects.Clear();
        var locales = SubjectType switch
        {
            ConditionSubjectType.Receipt => Constants.ConditionReceiptSubjects,
            ConditionSubjectType.Position => Constants.ConditionPositionSubjects,
            _ => []
        };
        foreach (var locale in locales)
            LocaleSubjects.Add(locale);
    }

    private void UpdateLocaleProps()
    {
        LocaleProps.Clear();
        var locales = SubjectType == ConditionSubjectType.Position || Subject > 0
            ? Constants.ConditionPositionProperties
            : Constants.ConditionReceiptProperties;
        foreach (var locale in locales)
            LocaleProps.Add(locale);
    }

    private void UpdateLocaleOperators()
    {
        LocaleOperators.Clear();
        var locales = ValueType switch
        {
            ConditionValueType.Flag => Constants.ConditionFlagsOperators,
            ConditionValueType.Enum => Constants.ConditionEnumsOperators,
            ConditionValueType.Text => Constants.ConditionTextOperators,
            ConditionValueType.Boolean => Constants.ConditionBooleanOperators,
            ConditionValueType.Float or ConditionValueType.DateTime or ConditionValueType.Integer 
                => Constants.ConditionComparableOperators,
            _ => []
        };
        foreach (var locale in locales)
            LocaleOperators.Add(locale);
    }

    private void UpdateValueControl()
    {
        var binding = new Binding(nameof(Value))
        {
            Source = this,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };
        switch (ValueType)
        {
            case ConditionValueType.Text:
                ValueControl = new TextBox();
                ValueControl.SetBinding(TextBox.TextProperty, binding);
                break;
            case ConditionValueType.Integer:
                ValueControl = new IntegerUpDown();
                ValueControl.SetBinding(IntegerUpDown.ValueProperty, binding);
                break;
            case ConditionValueType.DateTime:
                ValueControl = new DatePicker();
                ValueControl.SetBinding(DatePicker.SelectedDateProperty, binding);
                break;
            case ConditionValueType.Float:
                ValueControl = new DoubleUpDown();
                ValueControl.SetBinding(DoubleUpDown.ValueProperty, binding);
                break;
            case ConditionValueType.Flag:
            case ConditionValueType.Enum:
                ValueControl = new ComboBox
                {
                    DisplayMemberPath = nameof(EnumLocale<>.Locale),
                    SelectedValuePath = nameof(EnumLocale<>.Value),
                };
                ((ComboBox)ValueControl).ItemsSource = Prop switch
                {
                    ConditionProp.ReceiptType => Constants.ReceiptTypeLocales,
                    ConditionProp.Taxation => Constants.TaxationSystemLocales,
                    ConditionProp.TaxRate => Constants.TaxRateLocales,
                    ConditionProp.PositionType => Constants.PositionTypeLocales,
                    ConditionProp.Unit => Constants.PositionUnitLocales,
                    ConditionProp.PaymentMethod => Constants.PaymentMethodLocales,
                    _ => Array.Empty<object>()
                };
                ValueControl.SetBinding(ComboBox.SelectedIndexProperty, binding);
                break;
            default:
                ValueControl = null;
                return;
        }
        
        ValueControl?.MinWidth = 100;
        ValueControl?.MaxWidth = 250;
    }

    internal Expression<Predicate<object>> ToExpression()
    {
        throw new NotImplementedException();
    }

    private ConstantExpression? GetValueExpression()
    {
        var constType = this switch
        {
            { ValueType: ConditionValueType.Flag, Prop: ConditionProp.Taxation } => typeof(TaxationSystem),
            { ValueType: ConditionValueType.Enum, Prop: ConditionProp.ReceiptType } => typeof(Operation),
            { ValueType: ConditionValueType.Enum, Prop: ConditionProp.PaymentMethod } => typeof(PayMethod),
            { ValueType: ConditionValueType.Enum, Prop: ConditionProp.PositionType } => typeof(PositionType),
            { ValueType: ConditionValueType.Enum, Prop: ConditionProp.TaxRate } => typeof(TaxRate),
            { ValueType: ConditionValueType.Enum, Prop: ConditionProp.Unit } => typeof(MeasureUnit),
            { ValueType: ConditionValueType.Text } => typeof(string),
            { ValueType: ConditionValueType.Boolean } => null,
            { ValueType: ConditionValueType.Float } => typeof(ulong),
            { ValueType: ConditionValueType.DateTime } => typeof(DateTime),
            { ValueType: ConditionValueType.Integer } => typeof(uint),
            _ => typeof(object)
        };
        return constType is null ? null : Expression.Constant(Value, constType);
    }

    private Expression? GetOperatorExpression(Expression property, ConstantExpression value) => Operator switch
    {
        ConditionOperator.StartsWith => StringExpressions.StartsWith(property, value, false),
        ConditionOperator.StartsWithIgnoreCase => StringExpressions.StartsWith(property, value, true),
        ConditionOperator.Contains => StringExpressions.Contains(property, value, false),
        ConditionOperator.ContainsIgnoreCase => StringExpressions.Contains(property, value, true),
        ConditionOperator.EndsWith => StringExpressions.EndsWith(property, value, false),
        ConditionOperator.EndsWithIgnoreCase => StringExpressions.EndsWith(property, value, true),
        
        ConditionOperator.Equal => Expression.Equal(property, value),
        ConditionOperator.NotEqual => Expression.NotEqual(property, value),
        ConditionOperator.LessThan => Expression.LessThan(property, value),
        ConditionOperator.LessThanOrEqual => Expression.LessThanOrEqual(property, value),
        ConditionOperator.GreaterThan => Expression.GreaterThan(property, value),
        ConditionOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, value),
        
        ConditionOperator.IsTrue => Expression.IsTrue(property),
        ConditionOperator.IsFalse => Expression.IsFalse(property),
        
        _ => null
    };
}
