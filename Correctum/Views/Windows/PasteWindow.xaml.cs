using Correctum.ViewModels.Windows;

namespace Correctum.Views.Windows;

partial class PasteWindow
{
    public PasteWindow(string? text = null)
    {
        DataContext = new PasteWindowViewModel(text);
        InitializeComponent();
    }
}
