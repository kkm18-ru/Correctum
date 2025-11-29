using Tomlyn;
using RetailCorrector.Utils.Configurations;
using System.IO;

namespace RetailCorrector.Utils
{
    internal partial class Configuration
    {
        public static Configuration Instance
        {
            get
            {
                if(field is null)
                {
                    if(File.Exists(FileName) && Toml.TryToModel<Configuration>(File.ReadAllText(FileName), out var x, out _))
                        field = x;
                    else field = new Configuration();
                }
                return field;
            }
        }

        private const string FileName = "config.toml";

        public SqliteSection Sqlite { get; init; } = new();
        public HttpClientSection HttpClient { get; init; } = new();
        public ExportersSection Exporters { get; init; } = new();

        public void Save() => File.WriteAllText(FileName, Toml.FromModel(this));
    }
}
