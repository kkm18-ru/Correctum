using RetailCorrector.Data;
using RetailCorrector.Utils.Integrations.Clients;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Importers
{
    partial class FreeOfdRu : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public string Login
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                NotifyPropertyChanged();
            }
        } = string.Empty;

        public string Password
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                NotifyPropertyChanged();
            }
        } = string.Empty;

        public DateTime Begin
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                NotifyPropertyChanged();
            }
        } = DateTime.Now;

        public DateTime End
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                NotifyPropertyChanged();
            }
        } = DateTime.Now;
        public bool Authorizing
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                NotifyPropertyChanged();
            }
        } = false;
        public bool Authorized
        {
            get;
            set
            {
                if (field == value) return;
                field = value;
                NotifyPropertyChanged();
            }
        } = false;

        public bool CanAuthorize => !Authorizing;

        public ObservableCollection<FreeOfdRuDevice> Devices { get; } = [];
        public ObservableCollection<FreeOfdRuDevice> SelectedDevices { get; } = [];
    }
}
