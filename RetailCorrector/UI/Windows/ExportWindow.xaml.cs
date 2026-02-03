using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using RetailCorrector.Exporters;
using RetailCorrector.UI.Windows.Types;

namespace RetailCorrector.UI.Windows;

public partial class ExportWindow : DialogWindow, INotifyPropertyChanged
{
    public string Path { get; set => SetField(ref field, value); } = "";
    private readonly IExporter _exporter;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ExportWindow(IExporter exporter)
    {
        _exporter = exporter;
        InitializeComponent();
        _exporter.InitVisual((Grid)Content);
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
        if (_exporter.FileFilter is null)
        {
            var dialog = new OpenFolderDialog();
            if(dialog.ShowDialog() ?? false)
                Path = dialog.FolderName;
        }
        else
        {
            var dialog = new SaveFileDialog() 
            { 
                Filter = _exporter.FileFilter,
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
            await _exporter.Export(Path);
            MessageBox.Show("Чеки сохранены");
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Не удалось выполнить экспорт!", 
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}