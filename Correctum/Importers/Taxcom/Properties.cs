using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Correctum.Utils;

namespace Correctum.Importers;

partial class Taxcom : INotifyPropertyChanged
{
    public string Login
    {
        get;
        set => SetField(ref field, value);
    } = "";

    public string Password
    {
        get => SecretHelper.DecodeSecret(field);
        set => SetField(ref field, SecretHelper.EncodeSecret(value));
    } = "";

    public string Agreement
    {
        get;
        set => SetField(ref field, value);
    } = "";

    public bool Authorizing
    {
        get;
        set
        {
            if (SetField(ref field, value))
                OnPropertyChanged(nameof(CanAuthorized));
        }
    } = false;

    public bool Authorized
    {
        get;
        set => SetField(ref field, value);
    } = false;

    public bool CanAuthorized => !Authorizing;

    public DateRange DateRange { get; } = new();
    public ObservableCollection<string> Devices { get; } = [];
    public ObservableCollection<string> SelectedDevices { get; } = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}