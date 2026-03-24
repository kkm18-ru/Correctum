using CommunityToolkit.Mvvm.ComponentModel;

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

    public void GoToIndex() => CurrentIndex = ToIndex;
    public void OpenSettings() { }
    public void SwitchConnection() { }
    public void Run() { }
    public void RunTest() { }
}
