using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using RetailCorrector.UI.Windows.Types;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations;

namespace RetailCorrector.UI.Windows;

public sealed partial class ConfigurationWindow : DialogWindow
{
    public string PortEoU
    {
        get => $"COM{EoU.CurrentPort}";
        set => EoU.CurrentPort = byte.Parse(value[3..]);
    }
    
    public ConfigurationWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        Configuration.Instance.Save();
        Utils.Configurations.LogSection.ReloadLoggingConfiguration(true);
        base.OnClosing(e);
    }

    private void OnLokiPasswordLoaded(object sender, RoutedEventArgs e) => 
        ((PasswordBox)sender).Password = Configuration.Instance.Logging.LokiPassword;

    private void OnLokiPasswordChanged(object sender, RoutedEventArgs e) =>
        Configuration.Instance.Logging.LokiPassword = ((PasswordBox)sender).Password;

    private void OnBrowserPathSelector(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Исполняемый файл|*.exe",
            InitialDirectory = 
                Path.GetDirectoryName(Configuration.Instance.HttpClient.HeadlessBrowserPath)
        };
        if (!dialog.ShowDialog() ?? false) return;
        Configuration.Instance.HttpClient.HeadlessBrowserPath = dialog.FileName;
    }
}