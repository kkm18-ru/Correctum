using System.Reflection;
using RetailCorrector.Attributes;
using RetailCorrector.Importers;

namespace RetailCorrector.Data
{
    internal readonly struct ImportSessionInfo
    {
        public Guid Id { get; }
        public string Name { get; }
        public DateTime Imported { get; }
        public (string, object)[] Arguments { get; }

        public ImportSessionInfo(string name)
        {
            Name = name;
            Arguments = [];
            Id = Guid.CreateVersion7(); 
            Imported = DateTime.Now; 
        }
        
        public ImportSessionInfo(ImporterBase importer)
        {
            Id = Guid.CreateVersion7();
            Name = importer.DisplayName;
            Imported = DateTime.Now; 
            
            var type = importer.GetType();
            var properties = type.GetProperties()
                .Where(p => p.GetCustomAttribute<ImporterPropertyAttribute>() is not null).ToArray();
            Arguments = new (string, object)[properties.Length];
            for (var i = 0; i < Arguments.Length; i++)
                Arguments[i] = (
                    properties[i].GetCustomAttribute<ImporterPropertyAttribute>()?.Name ?? properties[i].Name,
                    properties[i].GetValue(importer, null)!);
        }

        public override string ToString() => $"Выгрузка {Id:P} \"{Name}\" от {Imported:yyyy'-'MM'-'dd' 'HH':'mm':'ss}";
    }
}
