using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Controls;

namespace RetailCorrector.UI.Controls;

public partial class ConditionItem : ListBoxItem, INotifyPropertyChanged
{
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
        }
    } = ConditionSubject.None;

    public ObservableCollection<string> LocaleSubjects { get; } = [];
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly PropertyChangedEventArgs SubjectTypeChangedArgs = new(nameof(SubjectType));
    private static readonly PropertyChangedEventArgs SubjectChangedArgs = new(nameof(Subject));
    
    public ConditionItem()
    {
        InitializeComponent();
        UpdateLocaleSubjects();
    }

    private void UpdateLocaleSubjects()
    {
        LocaleSubjects.Clear();
        switch (SubjectType)
        {
            case ConditionSubjectType.Receipt:
                LocaleSubjects.Add("В чеке");
                LocaleSubjects.Add("В любой позиции");
                LocaleSubjects.Add("Во всех позициях");
                break;
            case ConditionSubjectType.Position:
                LocaleSubjects.Add("В позиции");
                break;
        }
    }

    internal Expression<Predicate<object>> ToExpression()
    {
        throw new NotImplementedException();
    }
}

/// <summary>Тип субъекта условия</summary>
public enum ConditionSubjectType
{
    /// <summary>Чек</summary>
    Receipt,
    /// <summary>Позиция</summary>
    Position
}

/// <summary>Субъект условия</summary>
public enum ConditionSubject
{
    None = -1,
    /// <summary>Реквизит чека/позиции</summary>
    Default,
    /// <summary>Реквизит хотя бы одного из позиций</summary>
    AnyPosition,
    /// <summary>Реквизит всех позиций</summary>
    AllPositions
}
