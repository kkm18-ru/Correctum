using System.Windows.Controls;

namespace RetailCorrector.Controls
{
    internal partial class ActionMenuButton : Button
    {
        public ActionMenuButton()
        {
            Content = "  Действия  ";
        }

        protected override void OnClick()
        {
            new ActionMenu().IsOpen = true;
            base.OnClick();
        }
    }
}
