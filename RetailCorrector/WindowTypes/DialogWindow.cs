using System.Windows;
using System.Windows.Input;

namespace RetailCorrector.WindowTypes
{
    internal abstract class DialogWindow : Window
    {
        public RoutedCommand CloseCommand { get; } = 
            new(nameof(CloseCommand), typeof(DialogWindow));

        protected DialogWindow()
        {
            CloseCommand.InputGestures.Add(new KeyGesture(Key.Escape));
            CommandBindings.Add(new CommandBinding(CloseCommand, (_, _) => Close()));
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
            WindowStyle = WindowStyle.ToolWindow;
            ResizeMode = ResizeMode.NoResize;
        }
    }
}
