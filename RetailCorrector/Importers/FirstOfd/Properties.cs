using RetailCorrector.Utils;
using RetailCorrector.Utils.Integrations.Clients;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers;

partial class FirstOfd : INotifyPropertyChanged
{
    public string Login { get; set => SetValue(ref field, value); } = "";
    public string Password
    {
        get => SecretHelper.DecodeSecret(field);
        set => SetValue(ref field, SecretHelper.EncodeSecret(value));
    } = "";
    public bool IsAuthorized { get; set => SetValue(ref field, value); } = false;
    public bool Authorizing { get; set => SetValue(ref field, value); } = false;
    public DateRange DateRange { get; } = new();
    public ObservableCollection<FirstOfdKkt> Devices { get; } = [];
    public ObservableCollection<FirstOfdKkt> SelectedDevices { get; } = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private bool SetValue<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) 
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
