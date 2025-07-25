using System.Windows;
using System.Windows.Input;

namespace RetailCorrector.WindowTypes
{
    internal abstract class DialogWindow : Window
    {
        public RoutedCommand Escape { get; } = new(nameof(Escape), typeof(DialogWindow));

        protected DialogWindow()
        {
            Escape.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBindings.Add(new CommandBinding(Escape, (_, _) => Close()));
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
            WindowStyle = WindowStyle.ToolWindow;
            ResizeMode = ResizeMode.NoResize;
        }
    }
}
