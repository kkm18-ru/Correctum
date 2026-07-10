using Correctum.Utils.Integrations;
using Correctum.ViewModels.Windows;
using System.Windows;

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

    internal static void ShowDialog(FptrHandle fptr) =>
        Application.Current.Dispatcher.Invoke(() => new ExchangeOfdWindow(fptr).ShowDialog());
}
