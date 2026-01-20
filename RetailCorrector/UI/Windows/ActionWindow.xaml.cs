using RetailCorrector.UI.Controls;
using RetailCorrector.UI.Windows.Types;

namespace RetailCorrector.UI.Windows;

public partial class ActionWindow : DialogWindow
{
    private readonly ActionWindowMode _mode;
    public ConditionSubjectType SubjectType => (ConditionSubjectType)((int)_mode % 2);
    
    public ActionWindow(ActionWindowMode mode)
    {
        _mode = mode;
        InitializeComponent();
    }
}

public enum ActionWindowMode
{
    RemoveReceipt = 0,
    RemovePosition = 1,
    EditReceipt = 2,
    EditPosition = 3,
    CloneReceipt = 4,
    ClonePosition = 5,
}
