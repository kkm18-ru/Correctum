using Correctum.Importers;
using Correctum.UI.Controls;
using Correctum.ViewModels.Windows;
using System.Windows.Controls;

namespace Correctum.Views.Windows;

public partial class ImportWindow
{
    public UserControl ImportView { get; }
    internal ImportWindow(string importName, UserControl view)
    {
        ImportView = view;
        var vm = new ImportWindowViewModel(importName, (IImporter)view.DataContext);
        DataContext = vm;
        Closing += (_, _) => vm.OnClosing();
        vm.Imported += () =>
        {
            Close();
            ReceiptPanel.UpdateReceiptList();
        };
        InitializeComponent();
    }
    /*
#if DEBUG
    public ImportWindow()
    {
        ImportView = null!;
        var vm = new ImportWindowViewModel();
        DataContext = vm;
        Closing += (_, _) => vm.OnClosing();
        vm.Imported += () =>
        {
            Close();
            ReceiptPanel.UpdateReceiptList();
        };
        InitializeComponent();
    }
#endif*/
}
