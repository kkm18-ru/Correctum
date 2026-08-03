using Correctum.ViewModels.Windows;

namespace Correctum.Views.Windows;

public partial class FptrWindow
{
    public FptrWindow()
    {
        InitializeComponent();
        Closing += (s, e) => (DataContext as FptrWindowViewModel)?.OnClosing();
    }
}
