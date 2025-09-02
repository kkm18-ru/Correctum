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
            var usings = new StringBuilder();
            usings.AppendLine(
                """
                using Tomlyn.Model;
                using System.IO;
                using Tomlyn;
                """);
            builder.AppendLine("""
                               namespace RetailCorrector;

                               internal static class Configuration
                               {
                               """);
            loadMethod.AppendLine("""
                                      static Configuration()
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
                var lastIndex = fullname.LastIndexOf('.');
                usings.AppendLine($"using {fullname.Substring(0,lastIndex)};");
                var propName = typeSymbol.Name.Replace("Configuration", "");
                loadMethod.AppendLine(
                    $"        {propName} = {typeSymbol.Name}.Load(GetTable({typeSymbol.Name}.FileName));");
                builder.AppendLine($"    public static {typeSymbol.Name} {propName} {{ get; }}");
                AutoInterface(context, typeSymbol);
            }
            loadMethod.AppendLine("    }");
            usings.AppendLine();
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
            context.AddSource("Configuration.g.cs", usings.ToString() + builder.ToString());
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
