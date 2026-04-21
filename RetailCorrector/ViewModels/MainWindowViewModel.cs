using RetailCorrector.UI.Windows;
using RetailCorrector.Utils;
using System.Windows;
using System.Windows.Input;

namespace RetailCorrector.ViewModels
{
    public sealed class MainWindowViewModel : BaseViewModel
    {
        public ICommand PasteCommand { get; } = new RelayCommand(_ =>
        {
            if (!Clipboard.ContainsText()) return;
            new ParserWindow(Clipboard.GetText()).Show();
        });
    }
}
