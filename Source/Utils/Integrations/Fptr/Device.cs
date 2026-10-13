using Correctum.Exceptions;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using FnInfo = (string Model, string SerialNumber);

namespace Correctum.Utils.Integrations
{
    internal sealed partial class FptrHandle
    {
        public Version? ConfigurationVersion { get; private set; }
        public uint Model { get; private set; }
        public string FnModel { get; private set; }
        public string SerialNumber { get; private set; }
        public string FnSerialNumber { get; private set; }
        public uint DocFormat { get; private set; }

        public bool IsConnected => isOpened(handle);
        public int ErrorCode => errorCode(handle);
        public string ErrorDescription
        {
            get
            {
                var value = new byte[defaultStringSize * 2];
                var count = errorDescription(handle, value, defaultStringSize);
                if (count > defaultStringSize)
                {
                    value = new byte[count * 2];
                    errorDescription(handle, value, count);
                }
                return Encoding.Unicode.GetString(value, 0, count * 2).Trim('\0', ' ', '\t', '\n', '\r');
            }
        }


        public bool Connect()
        {
            if (open(handle) == 0)
            {
                DocFormat = GetFormatVersion();
                ConfigurationVersion = GetConfigurationVersion();
                Model = GetKktModel();
                SerialNumber = GetKktSerialNumber();
                (FnModel, FnSerialNumber) = GetFnInfo();
                return true;
            }
            return false;
        }

        public string GetSetting(string key)
        {
            var keyBytes = encoding.GetBytes(key);
            var bytes = new byte[sizeCharacter * defaultBytesSize];
            var size = getSingleSetting(handle, keyBytes, bytes, defaultBytesSize);
            if (size > defaultBytesSize)
            {
                bytes = new byte[sizeCharacter * size];
                getSingleSetting(handle, keyBytes, bytes, size);
            }
            return encoding.GetString(bytes, 0, size * sizeCharacter).TrimEnd(new char[1]);
        }
        
        public bool Disconnect()
        {
            DocFormat = 0;
            ConfigurationVersion = null;
            Model = 0;
            FnModel = "";
            SerialNumber = "";
            FnSerialNumber = "";
            return close(handle) == 0;
        }

        public bool ShowProperties(Window wind) =>
            showProperties(handle, 0, new WindowInteropHelper(wind).Handle) == 0;

        public bool LoginOperator(string fullname, string inn = "")
        {
            SetString(1021, fullname);
            SetString(1203, inn);
            return operatorLogin(handle) == 0;
        }

        private uint GetKktModel()
        {
            SetInteger(65587, 17); // DataType: ModelInfo
            return Query() ? GetInteger(65544) : 0u;
        }
        
        private string GetKktSerialNumber()
        {
            SetInteger(65587, 16); // DataType: SerialNumber
            return Query() ? GetString(65559) : "";
        }
        
        private FnInfo GetFnInfo()
        {
            SetInteger(65622, 2); // FNDataType: Info
            return QueryFN() ? (GetString(65874).TrimEnd('\0', ' '), GetString(65559)) : ("", "");
        }
        
        private uint GetFormatVersion()
        {
            SetInteger(65622, 7);
            if (!QueryFN()) return 0;
            var fn = GetInteger(65628);
            var dev = GetInteger(65629);
            return (fn, dev) switch
            {
                (0, _) => dev,
                (_, 0) => fn,
                (_, _) => Math.Min(fn, dev),
            };
        }

        /// <summary>
        /// Получает количество документов, ожидающих передачи оператору фискальных данных (ОФД).
        /// Для этого устанавливает тип данных ФН «Статус информационного обмена с ОФД»,
        /// запрашивает данные фискального накопителя и считывает количество неотправленных документов.
        /// </summary>
        /// <returns>
        /// Количество документов, ожидающих передачи в ОФД.
        /// </returns>
        public uint GetUnsentCount()
        {
            // todo "Исключить одновременное использование фискального дескриптора несколькими операциями и обеспечить корректное управление жизненным циклом устройства." 
            SetInteger(65622, 1);
            return QueryFN() ? GetInteger(65625) : throw new FptrException(this);
        }
    }
}
