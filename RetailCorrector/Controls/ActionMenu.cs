using System.Windows.Controls;

namespace RetailCorrector.Controls
{
    internal class ActionMenuButton : Button
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

    file class ActionMenu : ContextMenu
    {
        public ActionMenu()
        {

        }
    }
}
