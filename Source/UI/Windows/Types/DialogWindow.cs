using Correctum.Utils;
using System.Windows;
using System.Windows.Input;

namespace Correctum.UI.Windows.Types;

public abstract class DialogWindow : Window
{
    protected DialogWindow()
    {
        InputBindings.Add(new KeyBinding(new RelayCommand(Close), new(Key.Escape)));
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Owner = Application.Current.MainWindow;
        WindowStyle = WindowStyle.ToolWindow;
        ResizeMode = ResizeMode.NoResize;
    }
}
