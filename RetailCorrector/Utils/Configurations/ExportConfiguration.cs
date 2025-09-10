namespace RetailCorrector.Configurations
{
    internal partial struct ExportConfiguration() :
        IConfigurationSection<ExportConfiguration>
    {
        public int CountDocsPerFile { get; set; } = 25;
    }
}
