using System.Windows;
using Correctum.UI.Windows.Types;
using Correctum.ViewModels.Exports;

namespace Correctum.Views.Windows;

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