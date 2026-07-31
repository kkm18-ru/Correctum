using Correctum.Abstractions;
using Correctum.ViewModels.Windows;
using System.Windows;

namespace Correctum.Views.Windows;

public partial class ImportWindow
{
    internal ImportWindow(string importName, BaseImport import)
    {
        InitializeComponent();
        var vm = new ImportWindowViewModel(importName, import, progressBar.Progress);
        DataContext = vm;
        Closing += (_, _) => vm.OnClosing();
        vm.Imported += () =>
        {
            Close();
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).UpdatePanel();
        };
    }
}
