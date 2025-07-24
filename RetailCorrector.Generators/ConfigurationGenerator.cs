using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RetailCorrector.Generators
{
    [Generator]
    internal class ConfigurationGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var prov = context.SyntaxProvider.CreateSyntaxProvider(
                (n, _) => n is StructDeclarationSyntax,
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(prov.Collect(), Execute);
        }

        private void Execute(SourceProductionContext ctx, ImmutableArray<ISymbol> symbols)
        {
            var builder = new StringBuilder();
            var loadMethod = new StringBuilder();
            builder.AppendLine("""
                               using RetailCorrector.ConfigurationSections;
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
            foreach(var symb in symbols)
            {
                var typeSymbol = (ITypeSymbol)symb;
                if (typeSymbol.Interfaces.All(i => i.ToDisplayString() == 
                    "RetailCorrector.Configurations.IConfigurationSection")) continue;
                var fullname = symb.ToDisplayString();
                if (!typeSymbol.Name.EndsWith("Configuration")) continue;
                var propName = typeSymbol.Name.Replace("Configuration", "");
                loadMethod.AppendLine(
                    $"        {propName} = {typeSymbol.Name}.Create(GetTable(\"{propName}\"));");
                builder.AppendLine($"    public static {typeSymbol.Name} {propName} {{ get; }}");
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
            ctx.AddSource("Configurations.g.cs", builder.ToString());
        }
    }
}
