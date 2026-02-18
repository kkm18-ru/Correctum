using System.ComponentModel;
using System.Runtime.CompilerServices;
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
}