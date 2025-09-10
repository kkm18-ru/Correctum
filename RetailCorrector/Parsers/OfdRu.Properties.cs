using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace RetailCorrector.Parsers
{
    internal partial class OfdRu : INotifyPropertyChanged
    {
        private static byte COUNT_TRIES => Configuration.OfdRu.CountTries;
        private static int REQUEST_DELAY => Configuration.OfdRu.RequestDelay;

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
        public string Vatin
        {
            get => _vatin;
            set
            {
                if (_vatin == value) return;
                _vatin = value;
                OnPropertyChanged();
            }
        }
        public string DeviceId
        {
            get => _deviceId;
            set
            {
                if (_deviceId == value) return;
                _deviceId = value;
                OnPropertyChanged();
            }
        }
        public string Token
        {
            get => _token;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    MessageBox.Show("Ввод аутентификационного токена обязателен",
                        "Введенный аутентификационный токен некорректен", 0, MessageBoxImage.Error);
                    return;
                }
                if (_token == value) return;
                _token = value;
                OnPropertyChanged();
            }
        }

        private DateTime _begin = DateTime.Today;
        private DateTime _end = DateTime.Today;
        private string _vatin = "";
        private string _deviceId = "";
        private string _token = "";

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
