namespace RetailCorrector.Utils.Configurations
{
    internal partial struct SabyConfiguration() :
        IConfigurationSection<SabyConfiguration>
    {
        public static string FileName => "saby";
        public long CountDocsByRequest { get; set; } = 100;
    }
}
