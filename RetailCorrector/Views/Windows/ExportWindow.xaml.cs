using System.Windows;
using RetailCorrector.UI.Windows.Types;
using RetailCorrector.ViewModels.Exports;

namespace RetailCorrector.Views.Windows;

public partial class ExportWindow : DialogWindow
{
    public ExportWindow(BaseExportViewModel vm)
    {
        vm.NotifyCreated += 
            (msg, title, btn, img) => 
                MessageBox.Show(msg, title, btn, img);
        DataContext = vm;
        InitializeComponent();
    }
}