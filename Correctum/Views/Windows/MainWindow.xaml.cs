using Correctum.Utils.Integrations;
using System.Windows;

namespace Correctum.Views.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    protected override void OnClosed(EventArgs e)
    {
        FptrHandle.FreeLibrary();
        base.OnClosed(e);
    }
}
