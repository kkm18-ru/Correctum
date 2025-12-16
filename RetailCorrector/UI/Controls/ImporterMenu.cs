using System.Windows.Controls;
using RetailCorrector.Utils;

namespace RetailCorrector.UI.Controls
{
    internal class ImporterMenuButton : Button
    {
        public ImporterMenuButton()
        {
            Content = "  Парсеры  ";
        }

        protected override void OnClick()
        {
            var menu = new ContextMenu
            {
                IsOpen = true
            };
            foreach (var action in ModuleRegistry.Importers)
                menu.Items.Add(new MenuItem
                {
                    Header = action.Name,
                    Command = new RelayCommand(_ => action.Show())
                });
            base.OnClick();
        }
    }
}
