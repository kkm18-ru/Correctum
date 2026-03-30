using Avalonia.Controls;
using Correctum.ViewModels.Windows;

namespace Correctum.Views.Windows;

public partial class OpenFileDialog : Window
{
    public OpenFileDialog(FileType[] types, string? basePath = null, string title = "Открытие файла")
    {
        Title = title;
        basePath ??= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        DataContext = new OpenFileDialogViewModel(types, basePath);
        InitializeComponent();
    }

    private void OpenFile(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var vm = DataContext as OpenFileDialogViewModel;
        Close((vm?.AbsolutePath ?? string.Empty, vm?.FileTypeIndex ?? -1));
    }

    private void Cancel(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();
}