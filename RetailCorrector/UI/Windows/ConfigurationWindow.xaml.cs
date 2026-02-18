using System.ComponentModel;
using System.Runtime.CompilerServices;
using RetailCorrector.UI.Windows.Types;
using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations;

namespace RetailCorrector.UI.Windows;

public sealed partial class ConfigurationWindow : DialogWindow, INotifyPropertyChanged
{
    public string PortEoU
    {
        get; set
        {
            if (!SetField(ref field, value)) return;
            EoU.CurrentPort = byte.Parse(PortEoU[3..]);
        }
    }
    
    public ConfigurationWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        Configuration.Instance.Save();
        Utils.Configurations.LogSection.ReloadLoggingConfiguration();
        base.OnClosing(e);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}