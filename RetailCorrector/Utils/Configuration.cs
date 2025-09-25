using RetailCorrector.Attributes;
using Tomlyn.Model;
using Tomlyn;
using System.IO;

namespace RetailCorrector.Utils
{
    [Singleton]
    internal partial class Configuration
    {
        private const string FileName = "config.toml";
        
        [NotifyChanged(DefaultValue = 1000)] public partial int HttpRequestDelay { get; set; }
        [NotifyChanged(DefaultValue = 3)] public partial byte HttpTriesCount { get; set; }
        [NotifyChanged(DefaultValue = 100)] public partial short HttpPageSize { get; set; }

        [NotifyChanged(DefaultValue = 100)] public partial int SqlLimit { get; set; }

        [NotifyChanged(DefaultValue = 100)] public partial int FptrExportDocsPerFile { get; set; }

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
