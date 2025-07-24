using Tomlyn.Model;

namespace RetailCorrector.ConfigurationSections
{
    internal interface IConfigurationSection<T>
        where T : struct, IConfigurationSection<T>
    {
        abstract static T Create(TomlTable? table);
    }
}
