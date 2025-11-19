using Tomlyn;
using RetailCorrector.Utils.Configurations;
using System.IO;

namespace RetailCorrector.Utils
{
    internal partial class Configuration
    {
        private static Configuration _instance = null!;
        public static Configuration Instance => 
            _instance ??= Toml.ToModel<Configuration>(File.ReadAllText(FileName));

        private const string FileName = "config.toml";

        public SqliteSection Sqlite { get; set; }
        public HttpClientSection HttpClient { get; set; }
        public ExportersSection Exporters { get; set; }
        public FptrSection Fptr { get; set; }

        public void Save() => File.WriteAllText(FileName, Toml.FromModel(this));
    }
}
