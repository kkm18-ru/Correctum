using CommunityToolkit.Mvvm.ComponentModel;

namespace Correctum.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public string Greeting { get; } = "Welcome to Avalonia!";
    }
}
