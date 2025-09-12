using System.Windows.Controls;

namespace RetailCorrector.UI.Controls
{
    internal class FptrButton : Button
    {
        public FptrButton()
        {
            Content = "  ККТ-драйвер  ";
        }

        protected override void OnClick()
        {
            new Windows.FptrWindow().ShowDialog();
            base.OnClick();
        }
    }
}
