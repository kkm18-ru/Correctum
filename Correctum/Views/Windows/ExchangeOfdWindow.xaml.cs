using Correctum.Utils.Integrations;
using Correctum.ViewModels.Windows;

namespace Correctum.Views.Windows;

public partial class ExchangeOfdWindow
{
    internal ExchangeOfdWindow(FptrHandle fptr)
    {
        ExchangeOfdWindowViewModel viewModel;
        DataContext = viewModel = new ExchangeOfdWindowViewModel(fptr);
        viewModel.Completed += () => Dispatcher.Invoke(Close);
        InitializeComponent();
    }
}
