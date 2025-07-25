using Tomlyn.Model;

namespace RetailCorrector.ConfigurationSections
{
    internal struct OfdRuConfiguration() : IConfigurationSection<OfdRuConfiguration>
    {
        /// <summary>
        /// Задержка между запросами (в миллисекундах)
        /// </summary>
        public ushort RequestDelay { get; set; } = 1000;

        public static OfdRuConfiguration Create(TomlTable? table)
        {
            var conf = new OfdRuConfiguration();
            if (table is not null)
            {
                conf.RequestDelay = (ushort)table["RequestDelay"];
            }
            return conf;
        }
    }
}
