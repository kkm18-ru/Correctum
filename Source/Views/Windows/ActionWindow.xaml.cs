using Correctum.Abstractions;
using Correctum.ViewModels.Windows;
using System.Windows;

namespace Correctum.Views.Windows;

public partial class ActionWindow
{
    internal ActionWindow(string actionName, IAction action)
    {
        var vm = new ActionWindowViewModel(actionName, action);
        DataContext = vm;
        vm.Done += () =>
        {
            Close();
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).UpdatePanel();
        };
        InitializeComponent();
    }
}
