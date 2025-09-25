using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RetailCorrector.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class RegisterParsersGenerator : IIncrementalGenerator
    {
        const string ATTRIBUTE = "RetailCorrector.Attributes.RegisterParserAttribute";
        const string BASE = "RetailCorrector.Parsers.ParserBase";
        const string TEMPLATE = """
                            Items.Add(new MenuItem 
                            {{ 
                                Header = "{0}", 
                                Command = new RelayCommand(_ => {1}.Show()) 
                            }});
            """;

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var sources = context.SyntaxProvider.CreateSyntaxProvider(
                (n, _) => n is ClassDeclarationSyntax,
                (ctx, _) => (ITypeSymbol)ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(sources.Collect(), Generate);
        }

        private static void Generate(SourceProductionContext ctx, ImmutableArray<ITypeSymbol> arr)
        {
            var builder = new StringBuilder();
            builder.AppendLine(
                """
                using RetailCorrector.Utils;
                using System.Windows.Controls;

                namespace RetailCorrector.UI.Controls
                {
                    partial class ParserMenuButton
                    {
                        private class ParserMenu : ContextMenu
                        {
                            public ParserMenu()
                            {
                """);
            foreach (var symb in arr.Distinct<ITypeSymbol>(SymbolEqualityComparer.Default))
            {
                if (!symb.IsCommonClass()) continue;
                if (!symb.InheritedFrom(BASE)) continue;
                var attr = symb.GetAttributes().FirstOrDefault(a => a.AttributeClass.ToDisplayString() == ATTRIBUTE);
                if (attr is null) continue;
                var args = attr.NamedArguments.ToImmutableDictionary();
                builder.AppendLine(string.Format(TEMPLATE, args["DisplayName"].Value, symb.ToDisplayString()));
            }
            builder.AppendLine(
                """
                            }
                        }
                    }
                }
                """);
            ctx.AddSource("ParsersMenu.g.cs", builder.ToString());
        }
    }
}
