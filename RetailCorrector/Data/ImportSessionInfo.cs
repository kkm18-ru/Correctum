using System.Reflection;
using RetailCorrector.Attributes;
using RetailCorrector.Importers;

namespace RetailCorrector.Data;

public readonly struct ImportSessionInfo : IEquatable<ImportSessionInfo>
{
    public Guid Id { get; }
    public string Name { get; }
    public DateTime Imported { get; }
    public ImportSessionArgument[] Arguments { get; }

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
        Arguments = new ImportSessionArgument[properties.Length];
        for (var i = 0; i < Arguments.Length; i++)
            Arguments[i] = new ImportSessionArgument(properties[i], importer);
    }

    public override string ToString() => $"Выгрузка {Id:P} \"{Name}\" от {Imported:yyyy'-'MM'-'dd' 'HH':'mm':'ss}";

    public bool Equals(ImportSessionInfo other) => 
        Id.Equals(other.Id) && Name == other.Name && Imported.Equals(other.Imported) && Arguments.Equals(other.Arguments);
    public override bool Equals(object? obj) => obj is ImportSessionInfo other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Id, Name, Imported, Arguments);
    public static bool operator ==(ImportSessionInfo left, ImportSessionInfo right) => left.Equals(right);
    public static bool operator !=(ImportSessionInfo left, ImportSessionInfo right) => !(left == right);
}

public readonly struct ImportSessionArgument(PropertyInfo propertyInfo, ImporterBase importer)
{
    public string Name { get; } = 
        propertyInfo.GetCustomAttribute<ImporterPropertyAttribute>()?.Name ?? propertyInfo.Name;
    public object? Value { get; } = propertyInfo.GetValue(importer, null);
}
