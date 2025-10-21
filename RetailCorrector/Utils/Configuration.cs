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

        private Configuration()
        {
            if (File.Exists(FileName)) {
                var conf = Toml.ToModel(File.ReadAllText(FileName));

                var httpSection = (TomlTable)conf["HTTP"]!;
                HttpRequestDelay = (int)(long)httpSection["RequestDelay"];
                HttpTriesCount = (byte)(long)httpSection["TriesCount"];
                HttpPageSize = (short)(long)httpSection["PageSize"];

                var sqlSection = (TomlTable)conf["SQL"]!;
                SqlLimit = (int)(long)sqlSection["Limit"];

                var exportSection = (TomlTable)conf["Export"]!;
                var fptrExportSection = (TomlTable)exportSection["Fptr"]!;
                FptrExportDocsPerFile = (int)(long)fptrExportSection["DocsPerFile"];
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
                }
            };
            File.WriteAllText(FileName, Toml.FromModel(table));
        }
    }
}
