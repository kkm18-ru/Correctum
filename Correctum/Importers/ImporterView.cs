using System.Windows.Controls;
using Correctum.Utils;

namespace Correctum.Importers;

internal abstract class ImporterView<TV, TP> : UserControl
    where TV : ImporterView<TV, TP>, new()
    where TP : ImporterBase<TP, TV>, ISingleton<TP>, new()
{
    public TP Presenter => (TP)DataContext;

    protected ImporterView()
    {
        Margin = new System.Windows.Thickness(0, 0, 0, 5);
        DataContext = TP.Instance;
    }
}
