using Tomlyn.Model;
using Tomlyn;
using System.IO;
using System.ComponentModel;

namespace RetailCorrector.Utils
{
    internal partial class Configuration: INotifyPropertyChanged
    {
        private static Configuration _instance = null!;
        public static Configuration Instance => _instance ??= new Configuration();

        private const string FileName = "config.toml";

        #region Свойства
        public int HttpRequestDelay
        {
            get => _httpRequestDelay;
            set
            {
                if (value == _httpRequestDelay) return;
                _httpRequestDelay = value;
                PropertyChanged?.Invoke(this, HttpRequestDelayNotifier);
            }
        }
        public byte HttpTriesCount
        {
            get => _httpTriesCount;
            set
            {
                if (value == _httpTriesCount) return;
                _httpTriesCount = value;
                PropertyChanged?.Invoke(this, HttpTriesCountNotifier);
            }
        }
        public short HttpPageSize
        {
            get => _httpPageSize;
            set
            {
                if (value == _httpPageSize) return;
                _httpPageSize = value;
                PropertyChanged?.Invoke(this, HttpPageSizeNotifier);
            }
        }
        public int SqlLimit
        {
            get => _sqlLimit;
            set
            {
                if (value == _sqlLimit) return;
                _sqlLimit = value;
                PropertyChanged?.Invoke(this, SqlLimitNotifier);
            }
        }
        public int FptrExportDocsPerFile
        {
            get => _fptrExportDocsPerFile;
            set
            {
                if (value == _fptrExportDocsPerFile) return;
                _fptrExportDocsPerFile = value;
                PropertyChanged?.Invoke(this, FptrExportDocsPerFileNotifier);
            }
        }
        public int FptrWaitRebootTimeout
        {
            get => _fptrWaitRebootTimeout;
            set
            {
                if (value == _fptrWaitRebootTimeout) return;
                _fptrWaitRebootTimeout = value;
                PropertyChanged?.Invoke(this, FptrWaitRebootTimeoutNotifier);
            }
        }
        #endregion

        #region Переменные
        private int _httpRequestDelay = 1000;
        private byte _httpTriesCount = 3;
        private short _httpPageSize = 100;
        private int _sqlLimit = 100;
        private int _fptrExportDocsPerFile = 100;
        private int _fptrWaitRebootTimeout = 1500;

        #endregion

        #region Кеши
        private static readonly PropertyChangedEventArgs HttpRequestDelayNotifier = new(nameof(HttpRequestDelay));
        private static readonly PropertyChangedEventArgs HttpTriesCountNotifier = new(nameof(HttpTriesCount));
        private static readonly PropertyChangedEventArgs HttpPageSizeNotifier = new(nameof(HttpPageSize));
        private static readonly PropertyChangedEventArgs SqlLimitNotifier = new(nameof(SqlLimit));
        private static readonly PropertyChangedEventArgs FptrExportDocsPerFileNotifier = new(nameof(FptrExportDocsPerFile));
        private static readonly PropertyChangedEventArgs FptrWaitRebootTimeoutNotifier = new(nameof(FptrWaitRebootTimeout));
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        private Configuration()
        {
            if (!File.Exists(FileName)) return;
            var conf = Toml.ToModel(File.ReadAllText(FileName));

            if (conf.TryGetValue("HTTP", out var value) && value is TomlTable httpSection)
            {
                if (httpSection.TryGetValue("RequestDelay", out value) && value is long)
                    HttpRequestDelay = (int)(long)value;
                if (httpSection.TryGetValue("TriesCount", out value) && value is long)
                    HttpTriesCount = (byte)(long)value;
                if (httpSection.TryGetValue("PageSize", out value) && value is long)
                    HttpPageSize = (short)(long)value;
            }
            
            if (conf.TryGetValue("SQL", out value) && value is TomlTable sqlSection)
            {
                if (sqlSection.TryGetValue("Limit", out value) && value is long)
                    SqlLimit = (int)(long)value;
            }
            
            if (conf.TryGetValue("Export", out value) && value is TomlTable exportSection)
            {
                if (exportSection.TryGetValue("Fptr", out value) && value is TomlTable exportFptrSection)
                {
                    if (exportFptrSection.TryGetValue("DocsPerFile", out value) && value is long)
                        FptrExportDocsPerFile = (int)(long)value;
                }
            }
            
            if (conf.TryGetValue("Fptr", out value) && value is TomlTable fpreSection)
            {
                if (fpreSection.TryGetValue("WaitRebootTimeout", out value) && value is long)
                    FptrWaitRebootTimeout = (int)(long)value;
            }
        }

        public void Save()
        {
            var table = new TomlTable()
            {
                { 
                    "HTTP", new TomlTable()
                    {
                        { "RequestDelay", HttpRequestDelay },
                        { "TriesCount", HttpTriesCount },
                        { "PageSize", HttpPageSize }
                    } 
                },
                { 
                    "SQL", new TomlTable()
                    {
                        { "Limit", SqlLimit }
                    } 
                },
                {
                    "Export", new TomlTable()
                    {
                        { 
                            "Fptr", new TomlTable()
                            {
                                { "DocsPerFile", FptrExportDocsPerFile }
                            } 
                        }
                    }
                },
                {
                    "Fptr", new TomlTable()
                    {
                        { "WaitRebootTimeout", FptrWaitRebootTimeout }
                    } 
                }
            };
            File.WriteAllText(FileName, Toml.FromModel(table));
        }
    }
}
