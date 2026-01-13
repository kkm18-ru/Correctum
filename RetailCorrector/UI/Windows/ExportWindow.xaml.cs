using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using RetailCorrector.Exporters;
using RetailCorrector.UI.Windows.Types;
using RetailCorrector.Utils;

namespace RetailCorrector.UI.Windows;

public partial class ExportWindow : DialogWindow, INotifyPropertyChanged
{
    public string Path { get; set => SetField(ref field, value); } = "";
    public IExporter Exporter
    {
        get;
        set
        {
            if (!SetField(ref field, value)) return;
            var grid = (Grid)Content;
            if (grid.RowDefinitions.Count > 5)
            {
                grid.RowDefinitions.RemoveRange(5, grid.RowDefinitions.Count - 5);
                grid.Children.RemoveRange(6, grid.Children.Count - 6);
            }
            Path = "";
            Exporter.InitVisual(grid);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ExportWindow()
    {
        InitializeComponent();
        Exporter = ModuleRegistry.Exporters[0];
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    private void BrowsePath(object sender, RoutedEventArgs e)
    {
        if (Exporter.FileFilter is null)
        {
            var dialog = new OpenFolderDialog();
            if(dialog.ShowDialog() ?? false)
                Path = dialog.FolderName;
        }
        else
        {
            var dialog = new SaveFileDialog() 
            { 
                Filter = Exporter.FileFilter,
                AddExtension = true,
                OverwritePrompt = true,
            };
            if (dialog.ShowDialog() ?? false)
                Path = dialog.FileName;
        }
    }

    private async void Export(object sender, RoutedEventArgs e)
    {
        try
        {
            await Exporter.Export(Path);
            MessageBox.Show("Чеки сохранены");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Не удалось выполнить экспорт!", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}