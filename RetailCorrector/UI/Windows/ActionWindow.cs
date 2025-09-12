using RetailCorrector.Actions;
using System.Windows;
using System.Windows.Controls;

namespace RetailCorrector.UI.Windows
{
    internal class ActionWindow : Types.DialogWindow
    {
        public ActionWindow(UserControl view) :
            this((IAction)view.DataContext)
        {
            ((StackPanel)Content).Children.Insert(0, view);
        }

        public ActionWindow(IAction action)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            var stack = new StackPanel { Margin = new(5, 5, 5, 10) };
            var btn = new Button
            {
                Content = "  Выполнить  ",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new(0,5,0,-5)
            };
            btn.Click += (_, _) =>
            {
                action.Start();
                Close();
            };
            stack.Children.Add(btn);
            Content = stack;
        }
    }
}
