using Correctum.ViewModels.Windows;

namespace Correctum.Views.Windows;

partial class PasteWindow
{
    public PasteWindow(string? text = null)
    {
        var vm = new PasteWindowViewModel(text);
        DataContext = vm;
        vm.OnPasted += Close;
        InitializeComponent();
    }
}
