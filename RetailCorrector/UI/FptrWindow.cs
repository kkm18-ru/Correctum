using Atol.Drivers10.Fptr;
using RetailCorrector.UI.WindowTypes;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace RetailCorrector.UI
{
    internal class FptrWindow : DialogWindow
    {
        private readonly Fptr _driver;

        public FptrWindow()
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            _driver = new Fptr();
            var grid = new Grid
            {
                Margin = new Thickness(5)
            };

            grid.AddColumn(GridUnitType.Star);
            grid.AddColumn(GridUnitType.Pixel, 5);
            grid.AddColumn(GridUnitType.Star);

            grid.AddChild(CreateConfigurationButton(), dx: 3);

            Content = grid;
        }

        private Button CreateConfigurationButton()
        {
            var btn = new Button
            {
                Content = "  Настройка драйвера  "
            };
            btn.Click += (_, _) =>  _driver.showProperties(0, 
                new WindowInteropHelper(MainWindow.Instance).EnsureHandle());
            return btn;
        }

        protected override void OnClosed(EventArgs e)
        {
            _driver.close();
            _driver.destroy();
        }
    }
}
