using System.Windows.Controls;
using RetailCorrector.Utils;

namespace RetailCorrector.Actions
{
    internal abstract class ActionView<TV, TP> : UserControl
        where TV : ActionView<TV, TP>, new()
        where TP : ActionBase<TP, TV>, ISingleton<TP>, new()
    {
        protected TP Context => (TP)DataContext;

        protected ActionView()
        {
            DataContext = TP.Instance;
        }
    }
}
