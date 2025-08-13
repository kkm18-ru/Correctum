using System.Windows.Controls;

namespace RetailCorrector.Actions
{
    internal abstract class ActionView<TP> : UserControl
        where TP : ActionPresenter, new()
    {
        protected TP Context => (TP)DataContext;

        protected ActionView()
        {
            DataContext = new TP();
        }
    }
}
