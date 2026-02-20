using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;
using RetailCorrector.UI.Windows;

namespace RetailCorrector.UI.Controls;

public sealed class FilePathBox : DockPanel
{
    public string FilePath
    {
        get => (string)GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }

    public string FileFilter { get; set; } = string.Empty;

    public static readonly DependencyProperty FilePathProperty =
        DependencyProperty.Register(nameof(FilePath), typeof(string), typeof(FilePathBox), 
            new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    
    protected override void OnInitialized(EventArgs e)
    {
        var btn = new Button { Width = 20, Height = 20, Content = "..." };
        btn.Click += ShowOpenFileDialog;
        SetDock(btn, Dock.Right);
        Children.Add(btn);

        SetDock(Children[Children.Add(new Separator { Width = 5, Height = 0 })], Dock.Right);
        
        var box = new TextBox();
        box.SetBinding(TextBox.TextProperty, new Binding(nameof(FilePath)) { Source = this });
        Children.Add(box);
        
        base.OnInitialized(e);
    }

    private void ShowOpenFileDialog(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = FileFilter,
            Multiselect = false,
            FileName = FilePath
        };
        if (dialog.ShowDialog(MainWindow.Instance) ?? false)
            FilePath = dialog.FileName;
    }
}