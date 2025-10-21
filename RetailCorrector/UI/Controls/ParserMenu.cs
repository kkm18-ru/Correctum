using System.Windows.Controls;
using RetailCorrector.Utils;

namespace RetailCorrector.UI.Controls
{
    internal class ParserMenuButton : Button
    {
        public ParserMenuButton()
        {
            Content = "  Парсеры  ";
        }

        protected override void OnClick()
        {
            var menu = new ContextMenu
            {
                IsOpen = true
            };
            foreach (var action in ModuleRegistry.Parsers)
                menu.Items.Add(new MenuItem
                {
                    Header = action.Name,
                    Command = new RelayCommand(_ => action.Show())
                });
            base.OnClick();
        }
    }
}
