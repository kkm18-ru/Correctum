using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace RetailCorrector.Parsers.Taxcom
{
    internal partial class TaxcomPresenter : INotifyPropertyChanged
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

        private string _login = "";
        private string _passwd = "";

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
