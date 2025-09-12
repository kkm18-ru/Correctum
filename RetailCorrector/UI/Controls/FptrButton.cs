using System.Windows;
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
            try
            {
                new Windows.FptrWindow().ShowDialog();
                base.OnClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", 0, MessageBoxImage.Error);
            }
        }
    }
}
