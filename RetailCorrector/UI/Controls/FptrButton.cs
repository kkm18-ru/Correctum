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
            new FptrWindow().ShowDialog();
            base.OnClick();
        }
    }
}
