using Correctum.Utils.Integrations;
using System.IO;
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

    protected override async void OnDrop(DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop)
            && (string[])e.Data.GetData(DataFormats.FileDrop) is { Length: 1 } files
            && Path.GetExtension(files[0]) == StorageContext.FileExtension)
            StorageContext.SetContextPath(files[0]);
        base.OnDrop(e);
    }
}
