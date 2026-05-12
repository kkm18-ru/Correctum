using Correctum.Abstractions;
using Correctum.UI.Controls;
using Correctum.ViewModels.Windows;

namespace Correctum.Views.Windows;

public partial class ImportWindow
{
    internal ImportWindow(string importName, BaseImport import)
    {
        var vm = new ImportWindowViewModel(importName, import);
        DataContext = vm;
        Closing += (_, _) => vm.OnClosing();
        vm.Imported += () =>
        {
            Close();
            ReceiptPanel.UpdateReceiptList();
        };
        InitializeComponent();
    }
}
