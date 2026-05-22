using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Correctum.UI.Windows.Types;
using Correctum.Utils;
using Microsoft.Win32;

namespace Correctum.UI.Windows;

public sealed partial class ConfigurationWindow : DialogWindow
{
    public ConfigurationWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        Configuration.Current.Save();
        Configuration.Current.Logging.Apply(true);
        base.OnClosing(e);
    }

    private void OnLokiPasswordLoaded(object sender, RoutedEventArgs e) => 
        ((PasswordBox)sender).Password = Configuration.Current.Logging.LokiPassword;

    private void OnLokiPasswordChanged(object sender, RoutedEventArgs e) =>
        Configuration.Current.Logging.LokiPassword = ((PasswordBox)sender).Password;

    private void BrowseChromiumPath(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            InitialDirectory = Path.GetDirectoryName(Configuration.Current.Http.WebBrowserPath),
            Filter = "Исполняемый файл|*.exe",
            Multiselect = false,
        };
        if (!dialog.ShowDialog() ?? true) return;
        Configuration.Current.Http.WebBrowserPath = dialog.FileName;
    }
}