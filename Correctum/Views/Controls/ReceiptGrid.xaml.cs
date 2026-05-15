using Correctum.Imports;
using Correctum.ViewModels.Windows;
using System.IO;
using System.Windows;

namespace Correctum.Views.Controls;

partial class ReceiptGrid
{
    public ReceiptGrid()
    {
        InitializeComponent();
    }

    protected override async void OnDrop(DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
        var pathes = (string[])e.Data.GetData(DataFormats.FileDrop);
        try
        {
            foreach (var path in pathes)
            {
                switch (Path.GetExtension(path))
                {
                    case ".sqlite":
                        var import = new SqliteImport { Path = path };
                        await foreach(var receipt in import.ImportAsync(CancellationToken.None))
                            Workspace.Instance.Receipts.Add(receipt);
                        break;
                    default: return;
                }
            }
            ((MainWindowViewModel)Application.Current.MainWindow.DataContext).UpdatePanel();
        }
        catch (Exception ex) 
        {
            MessageBox.Show(ex.ToString(), ex.Message, 0, MessageBoxImage.Error);
        }
    }
}
