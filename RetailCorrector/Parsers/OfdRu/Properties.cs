using RetailCorrector.Validations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace RetailCorrector.Parsers.OfdRu
{
    internal partial class OfdRuPresenter : INotifyPropertyChanged
    {
        private static byte COUNT_TRIES => Configurations.OfdRu.CountTries;
        private static int REQUEST_DELAY => Configurations.OfdRu.RequestDelay;

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
                if (string.IsNullOrWhiteSpace(value))
                {
                    MessageBox.Show("Ввод ИНН организации обязателен", 
                        "Введенный ИНН некорректен", 0, MessageBoxImage.Error);
                    return;
                }
                if (_vatin == value) return;
                if (value.ValidINN() is ValidationResult valid && !valid)
                {
                    MessageBox.Show(valid, "Введенный ИНН некорректен", 0, MessageBoxImage.Error);
                    return;
                }
                _vatin = value;
                OnPropertyChanged();
            }
        }
        public string DeviceId
        {
            get => _deviceId;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    MessageBox.Show("Ввод регистрационного номера ККТ обязателен",
                        "Введенный регистрационный номер ККТ некорректен", 0, MessageBoxImage.Error);
                    return;
                }
                if (_deviceId == value) return;
                if (value.ValidKKTRegId() is ValidationResult valid && !valid)
                {
                    MessageBox.Show(valid, "Введенный регистрационный номер ККТ некорректен", 0, MessageBoxImage.Error);
                    return;
                }
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

        public int CountDays
        {
            get => _days;
            set
            {
                if (_days != 0) return;
                if (value <= 0) return;
                _days = value;
                OnPropertyChanged();
            }
        }
        public int CurrentDayNumber
        {
            get => _day;
            set
            {
                if (_day == value) return;
                _day = value;
                OnPropertyChanged();
            }
        }

        private DateTime _begin = DateTime.Today;
        private DateTime _end = DateTime.Today;
        private string _vatin = "";
        private string _deviceId = "";
        private string _token = "";
        private int _days = 1;
        private int _day = 0;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
