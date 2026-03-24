using Correctum.ViewModels.Importers;
using Correctum.ViewModels.Windows;

namespace Correctum.Views.Windows;

public partial class ImportWindow : DialogWindow
{
    public ImportWindow(ImporterViewModel importer)
    {
        ImportWindowViewModel vm;
        DataContext = vm = new ImportWindowViewModel(importer);
        vm.Close += Close;
        InitializeComponent();
    }
}