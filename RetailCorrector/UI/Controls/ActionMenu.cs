using System.Windows.Controls;
using RetailCorrector.Utils;

namespace RetailCorrector.UI.Controls
{
    internal class ActionMenuButton : Button
    {
        public ActionMenuButton()
        {
            Content = "  Действия  ";
        }

        protected override void OnClick()
        {
            var menu = new ContextMenu
            {
                IsOpen = true
            };
            foreach (var action in ModuleRegistry.Actions)
                menu.Items.Add(new MenuItem
                {
                    Header = action.Name,
                    Command = new RelayCommand(_ => action.Show())
                });
            base.OnClick();
        }
    }
}
