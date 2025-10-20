using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RetailCorrector.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class RegisterDeserializersGenerator : IIncrementalGenerator
    {
        const string ATTRIBUTE = "RetailCorrector.Attributes.RegisterDeserializerAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var source = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => node is ClassDeclarationSyntax,
                (ctx, _) => (ITypeSymbol)ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(source.Collect(), Generate);
        }

        private static void Generate(SourceProductionContext ctx, ImmutableArray<ITypeSymbol> arr)
        {
            var exporters = new StringBuilder();
            var displayNames = new StringBuilder();

            arr = [..arr.Where(i => i.GetAttributes().Any(ad => ad.AttributeClass?.ToDisplayString() == ATTRIBUTE))];
            foreach (var type in arr.Distinct(SymbolEqualityComparer.Default))
            {
                exporters.AppendLine($"            new {type.ToDisplayString()}(),");
                var name = type.GetAttributes().FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == ATTRIBUTE)
                    .NamedArguments.ToImmutableDictionary()["DisplayName"];
                displayNames.AppendLine($"            \"{name.Value}\",");
            }

            ctx.AddSource("Exporters.g.cs", $$"""
                using RetailCorrector.Deserializers;

                namespace RetailCorrector.UI.Windows
                {
                    internal partial class DeserializeWindow
                    {
                        public readonly static IReceiptDeserializer[] Deserializers = [
                {{exporters}}
                        ];
                
                        private readonly static string[] DisplayNames = [
                {{displayNames}}
                        ];
                    }
                }
                """);
        }
    }
}
