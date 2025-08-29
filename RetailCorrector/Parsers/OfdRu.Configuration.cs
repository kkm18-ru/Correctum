namespace RetailCorrector.Configurations
{
    internal partial struct OfdRuConfiguration() : 
        IConfigurationSection<OfdRuConfiguration>
    {
        public static string FileName => "ofdru";
        /// <summary>
        /// Задержка между запросами (в миллисекундах)
        /// </summary>
        public ushort RequestDelay { get; set; } = 1000;
        /// <summary>
        /// Количество попыток на один запрос
        /// </summary>
        public byte CountTries { get; set; } = 3;
    }
}
