using RetailCorrector.Attributes;
using RetailCorrector.UI.Windows;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RetailCorrector.UI.Controls
{
    [Singleton]
    internal partial class TitleBar : Canvas
    {
        private TitleBar()
        {
            Height = 29;
            Background = SystemColors.AccentColorBrush;

            var tb = new TextBlock
            {
                Text = "RETAILCORRECTOR",
                FontFamily = new("Free 3 of 9"),
                FontSize = 38,
                Foreground = new SolidColorBrush(SystemColors.AccentColor.Subtraction(9, 9, 9))
            };
            Canvas.SetLeft(tb, 5);
            Children.Add(tb);

            var btn = CreateButton('');
            btn.Click += (_,_) => MainWindow.Instance.Close();
            Canvas.SetRight(btn, 0);
            Children.Add(btn);
            
            btn = CreateButton('');
            btn.Click += (_, _) => new SettingsWindow().ShowDialog();
            Canvas.SetRight(btn, 45);
            Children.Add(btn);

            btn = CreateButton('');
            btn.Click += (_,_) => MainWindow.Instance.WindowState = WindowState.Minimized;
            Canvas.SetRight(btn, 90);
            Children.Add(btn);
        }

        private Button CreateButton(char text) => new()
        {
            Content = text,
            FontFamily = new FontFamily("HoloLens MDL2 Assets"),
            Height = 29,
            Width = 45,
            BorderThickness = new(0),
            Background = SystemColors.AccentColorBrush,
        };
    }
}
