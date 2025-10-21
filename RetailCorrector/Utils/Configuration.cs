using Kraig.Attributes;
using Tomlyn.Model;
using Tomlyn;
using System.IO;

namespace RetailCorrector.Utils
{
    [Singleton]
    internal partial class Configuration
    {
        private const string FileName = "config.toml";
        
        [NotifyChanged] private int _httpRequestDelay = 1000;
        [NotifyChanged] private byte _httpTriesCount = 3;
        [NotifyChanged] private short _httpPageSize = 100;

        [NotifyChanged] private int _sqlLimit = 100;

        [NotifyChanged] private int _fptrExportDocsPerFile = 100;
        
        [NotifyChanged] private int _fptrWaitRebootTimeout = 1500;

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
