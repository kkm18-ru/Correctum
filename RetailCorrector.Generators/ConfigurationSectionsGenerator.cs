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
            var prov = context.SyntaxProvider.CreateSyntaxProvider(
                (n, _) => n is StructDeclarationSyntax,
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(prov.Collect(), Register);
        }

        private static void Register(SourceProductionContext context, ImmutableArray<ISymbol> array)
        {
            var builder = new StringBuilder();
            var loadMethod = new StringBuilder();
            builder.AppendLine("""
                               namespace RetailCorrector;

                               internal static class Configuration
                               {
                               """);
            loadMethod.AppendLine("""
                                      static Configuration()
                                      {
                                          if (!System.IO.Directory.Exists("config"))
                                              System.IO.Directory.CreateDirectory("config");
                                  """);
            foreach (var symb in array)
            {
                var typeSymbol = (ITypeSymbol)symb;
                if (typeSymbol.Interfaces.All(i => !i.ToDisplayString().StartsWith("RetailCorrector.IConfigurationSection"))) continue;
                var propName = typeSymbol.Name.Replace("Configuration", "");
                loadMethod.AppendLine(
                    $"        {propName} = {typeSymbol.ToDisplayString()}.Load(GetTable(\"{propName.ToLower()}\"));");
                builder.AppendLine($"    public static {typeSymbol.ToDisplayString()} {propName} {{ get; }}");
                AutoInterface(context, typeSymbol);
            }
            loadMethod.AppendLine("    }");
            builder.AppendLine($$"""

                               {{loadMethod}}
                                   private static Tomlyn.Model.TomlTable? GetTable(string filename) 
                                   {
                                       var path = System.IO.Path.Combine("config", $"{filename}.toml");
                                       return System.IO.File.Exists(path) ? Tomlyn.Toml.ToModel(System.IO.File.ReadAllText(path)) : null;
                                   }
                               }
                               """);
            context.AddSource("Configuration.g.cs", builder.ToString());
        }

        private static void AutoInterface(SourceProductionContext context, ITypeSymbol symbol)
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
                        if (table is null) return conf;
                """);
            foreach(var member in symbol.GetMembers())
            {
                if (member is not IPropertySymbol prop) continue;
                if (prop.IsStatic) continue;
                var name = prop.Name;
                builder.AppendLine($"        conf.{prop.Name} = ({prop.Type.ToDisplayString()})table[\"{name}\"];");
            }
            builder.AppendLine(
                """
                        return conf;
                    }
                }
                """);
            context.AddSource($"{symbol.Name}.g.cs", builder.ToString());
        }     
    }
}
