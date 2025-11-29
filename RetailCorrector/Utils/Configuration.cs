using Tomlyn;
using RetailCorrector.Utils.Configurations;
using System.IO;

namespace RetailCorrector.Utils
{
    internal partial class Configuration
    {
        private static Configuration _instance = null!;
        public static Configuration Instance
        {
            get
            {
                if(_instance is null)
                {
                    if(File.Exists(FileName) && Toml.TryToModel<Configuration>(File.ReadAllText(FileName), out var x, out _))
                        _instance = x;
                    else _instance = new Configuration();
                }
                return _instance;
            }
        }

        private const string FileName = "config.toml";

        public SqliteSection Sqlite { get; init; } = new();
        public HttpClientSection HttpClient { get; init; } = new();
        public ExportersSection Exporters { get; init; } = new();
        public FptrSection Fptr { get; init; } = new();

        public void Save() => File.WriteAllText(FileName, Toml.FromModel(this));
    }
}
