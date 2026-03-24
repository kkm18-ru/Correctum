using System.Text;
using Correctum.Configurations;
using NLog;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Correctum.Extensions;

public static class ConfigurationExtensions
{
    extension(Configuration)
    {
        public static void Initialize()
        {
            if (!File.Exists("configuration.yml"))
            {
                Configuration.Current = new Configuration
                {
                    HttpClient = HttpClientConfigurationSection.Default,
                    Logging = LoggingConfigurationSection.Default,
                    Importers = ImportersConfigurationSection.Default
                };
                return;
            }

            var text = File.ReadAllText("configuration.yml", Encoding.Unicode);
            var deserializer = new StaticDeserializerBuilder(new ConfigurationYamlContext())
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Configuration.Current = deserializer.Deserialize<Configuration>(text);
            LogManager.InitializeConfiguration();
        }
        
        public static void Reload()
        {
            if (!File.Exists("configuration.yml"))
                throw new FileNotFoundException("Файл конфигурации не найден!");
            var text = File.ReadAllText("configuration.yml", Encoding.Unicode);
            var deserializer = new StaticDeserializerBuilder(new ConfigurationYamlContext())
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            Configuration.Current = deserializer.Deserialize<Configuration>(text);
            LogManager.ReloadConfiguration();
        }
        
        public static void Save()
        {
            var serializer = new StaticSerializerBuilder(new ConfigurationYamlContext())
                .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            var text = serializer.Serialize(Configuration.Current);
            File.WriteAllText("configuration.yml", text, Encoding.Unicode);
        }
    }
}


[YamlStaticContext]
[YamlSerializable(typeof(Configuration))]
[YamlSerializable(typeof(LoggingConfigurationSection))]
[YamlSerializable(typeof(LokiConfigurationSection))]
[YamlSerializable(typeof(HttpClientConfigurationSection))]
[YamlSerializable(typeof(ImportersConfigurationSection))]
public partial class ConfigurationYamlContext : StaticContext;