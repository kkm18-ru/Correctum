using RetailCorrector.Utils;
using RetailCorrector.WindowTypes;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.Actions
{
    internal class ActionWindow : DialogWindow
    {
        public ActionWindow(UserControl view)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            var stack = new StackPanel { Margin = new(5) };
            stack.Children.Add(view);
            var btn = new Button
            {
                Content = "  Выполнить  ",
                HorizontalAlignment = HorizontalAlignment.Center,
                Command = new RelayCommand(_ => ((IActionPresenter)view.DataContext).Start())
            };
            stack.Children.Add(btn);
            Content = stack;
        }
    }
}
