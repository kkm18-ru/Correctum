using System.ComponentModel;

namespace RetailCorrector.Importers;

internal partial class FxPOS : INotifyPropertyChanged
{
    public string Path
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, PathNotifier);
        }
    } = "";

    private static readonly PropertyChangedEventArgs PathNotifier = new(nameof(Path));

    public event PropertyChangedEventHandler? PropertyChanged;
}
