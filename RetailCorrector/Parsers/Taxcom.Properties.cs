using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace RetailCorrector.Parsers
{
    internal partial class Taxcom : INotifyPropertyChanged
    {
        public string Login
        {
            get => _login;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    MessageBox.Show("Ввод логина личного кабинета обязателен",
                        "Введенный логин личного кабинета некорректен", 0, MessageBoxImage.Error);
                    return;
                }
                if (_login == value) return;
                _login = value;
                OnPropertyChanged();
            }
        }
        public string Password
        {
            get => _passwd;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    MessageBox.Show("Ввод пароля личного кабинета обязателен",
                        "Введенный пароль личного кабинета некорректен", 0, MessageBoxImage.Error);
                    return;
                }
                if (_passwd == value) return;
                _passwd = value;
                OnPropertyChanged();
            }
        }
        public string DeviceId
        {
            get => _devId;
            set
            {
                if (_devId == value) return;
                _devId = value;
                OnPropertyChanged();
            }
        }
        public DateTime BeginDate
        {
            get => _begin;
            set
            {
                if (_begin == value) return;
                _begin = value;
                OnPropertyChanged();
            }
        }
        public DateTime EndDate
        {
            get => _end;
            set
            {
                if (_end == value) return;
                _end = value;
                OnPropertyChanged();
            }
        }

        private string _login = "";
        private string _passwd = "";
        private string _devId = "";
        private DateTime _begin = DateTime.Today;
        private DateTime _end = DateTime.Today;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
