using System.Windows.Controls;

namespace RetailCorrector.Actions
{
    internal abstract class ActionView<TV, TP> : UserControl
        where TV : ActionView<TV, TP>, new()
        where TP : ActionPresenter<TP, TV>, new()
    {
        protected TP Context => (TP)DataContext;

        protected ActionView()
        {
            DataContext = new TP();
        }
    }
}
