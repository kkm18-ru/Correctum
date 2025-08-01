using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RetailCorrector.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class ConfigurationSectionsGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(CreateInterface);
            context.RegisterPostInitializationOutput(CreateAttributes);
            var prov = context.SyntaxProvider.CreateSyntaxProvider(
                (n, _) => n is StructDeclarationSyntax,
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(prov.Collect(), Register);
        }

        private void Register(SourceProductionContext context, ImmutableArray<ISymbol> array)
        {
            var builder = new StringBuilder();
            var loadMethod = new StringBuilder();
            builder.AppendLine("""
                               using Tomlyn.Model;
                               using System.IO;
                               using Tomlyn;

                               namespace RetailCorrector;

                               internal static class Configurations
                               {
                               """);
            loadMethod.AppendLine("""
                                      static Configurations()
                                      {
                                          if (!Directory.Exists(DIRECTORY_NAME))
                                              Directory.CreateDirectory(DIRECTORY_NAME);
                                  """);
            foreach (var symb in array)
            {
                var typeSymbol = (ITypeSymbol)symb;
                if (typeSymbol.Interfaces.All(i => i.ToDisplayString() ==
                    "RetailCorrector.IConfigurationSection")) continue;
                var fullname = symb.ToDisplayString();
                var propName = typeSymbol.Name.Replace("Configuration", "");
                loadMethod.AppendLine(
                    $"        {propName} = {typeSymbol.ToDisplayString()}.Create(GetTable({typeSymbol.ToDisplayString()}.FileName));");
                builder.AppendLine($"    public static {typeSymbol.Name} {propName} {{ get; }}");
                AutoInterface(context, typeSymbol);
            }
            loadMethod.AppendLine("    }");
            builder.AppendLine($$"""

                                   private const string DIRECTORY_NAME = "Configurations";

                               {{loadMethod}}
                                   private static TomlTable? GetTable(string filename) 
                                   {
                                       var path = Path.Combine(DIRECTORY_NAME, $"{filename}.toml");
                                       return File.Exists(path) ? Toml.ToModel(File.ReadAllText(path)) : null;
                                   }
                               }
                               """);
            context.AddSource("Configurations.g.cs", builder.ToString());
        }

        private void AutoInterface(SourceProductionContext context, ITypeSymbol symbol)
        {
            var builder = new StringBuilder();
            builder.AppendLine(
                $$"""
                using Tomlyn.Model;

                namespace {{symbol.ContainingNamespace.ToDisplayString()}};

                internal partial struct {{symbol.Name}} 
                {
                    public static {{symbol.Name}} Load(TomlTable? table)
                    {
                        var conf = new {{symbol.Name}}();
                        if (table is not null)
                        {
                """);
            foreach(var member in symbol.GetMembers())
            {
                if (member is not IPropertySymbol prop) continue;
                if (prop.IsStatic) continue;
                var name = prop.Name;
                if (prop.GetAttributes().FirstOrDefault(a => a.AttributeClass.ToDisplayString() == "RetailCorrector.RenameConfPropertyAttribute") is { } attr)
                    name = (string)attr.ConstructorArguments[0].Value;
                builder.AppendLine($"            conf.{prop.Name} = ({prop.Type.ToDisplayString()})table[\"{name}\"];");
            }
            builder.AppendLine(
                """
                        }
                        return conf;
                    }
                }
                """);
            context.AddSource($"{symbol.Name}.g.cs", builder.ToString());
        }

        private void CreateInterface(IncrementalGeneratorPostInitializationContext ctx) =>
            ctx.AddSource("IConfigurationSection.g.cs",
                """
                using Tomlyn.Model;

                namespace RetailCorrector;

                internal interface IConfigurationSection<T>
                    where T : struct, IConfigurationSection<T>
                {
                    abstract static string FileName { get; }
                    abstract static T Load(TomlTable? table);
                    //TomlTable Save(table);
                }
                """);

        private void CreateAttributes(IncrementalGeneratorPostInitializationContext ctx) =>
            ctx.AddSource("Attributes.g.cs",
                """
                namespace RetailCorrector;

                [AttributeUsage(AttributeTargets.Property)]
                internal class RenameConfPropertyAttribute(string newName) : Attribute;
                """);

    }
}
