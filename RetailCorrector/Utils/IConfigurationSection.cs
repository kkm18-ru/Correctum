using Tomlyn.Model;

namespace RetailCorrector;

internal interface IConfigurationSection<T>
    where T : struct, IConfigurationSection<T>
{
    abstract static T Load(TomlTable? table);
    //abstract static TomlTable Save(T @struct);
}