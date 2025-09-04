using System.Windows.Controls;

namespace RetailCorrector.UI.Controls
{
    internal partial class ParserMenuButton : Button
    {
        public ParserMenuButton()
        {
            Content = "  Парсеры  ";
        }

        protected override void OnClick()
        {
            new ParserMenu().IsOpen = true;
            base.OnClick();
        }
    }
}
