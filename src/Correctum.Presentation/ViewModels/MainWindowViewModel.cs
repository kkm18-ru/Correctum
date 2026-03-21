using CommunityToolkit.Mvvm.ComponentModel;

namespace Correctum.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public string Greeting { get; } = "Welcome to Avalonia!";
    }
}
