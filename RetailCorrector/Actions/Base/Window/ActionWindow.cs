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
            var stack = new StackPanel { Margin = new(5, 5, 5, 10) };
            stack.Children.Add(view);
            var btn = new Button
            {
                Content = "  Выполнить  ",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new(0,5,0,-5)
            };
            btn.Click += (_, _) =>
            {
                ((IActionPresenter)view.DataContext).Start();
                Close();
            };
            stack.Children.Add(btn);
            Content = stack;
        }
    }
}
