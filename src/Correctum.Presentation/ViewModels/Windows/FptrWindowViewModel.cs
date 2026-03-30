using CommunityToolkit.Mvvm.ComponentModel;
using Correctum.Fptr10;

namespace Correctum.ViewModels.Windows;

public sealed partial class FptrWindowViewModel : ObservableObject
{
    [ObservableProperty] private bool m_byYourself = true;
    [ObservableProperty] private string m_numbAct = string.Empty;
    [ObservableProperty] private string m_cashierName = string.Empty;
    [ObservableProperty] private string m_cashierInn = string.Empty;
    [ObservableProperty] private bool m_isInternet = false;
    [ObservableProperty] private string m_webAddress = string.Empty;

    [ObservableProperty] private int m_currentIndex = 0;
    [ObservableProperty] private int m_toIndex = 0;

    private Fptr _driver = new();

    public void GoToIndex() => CurrentIndex = ToIndex;
    public void OpenSettings() => _driver.showProperties(Constants.LIBFPTR_GUI_PARENT_NATIVE, 0);
    public void SwitchConnection() { }
    public void Run() { }
    public void RunTest() { }
}
