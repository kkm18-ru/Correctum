namespace RetailCorrector.Configurations
{
    internal partial struct SqlConfiguration() :
        IConfigurationSection<SqlConfiguration>
    {
        public int LimitDocuments { get; set; } = 100;
    }
}
