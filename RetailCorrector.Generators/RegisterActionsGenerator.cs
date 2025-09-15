using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RetailCorrector.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class RegisterActionsGenerator : IIncrementalGenerator
    {
        const string ATTRIBUTE = "RetailCorrector.Attributes.RegisterActionAttribute";
        const string BASE = "RetailCorrector.Actions.ActionBase";
        const string TEMPLATE = """
                            Items.Add(new MenuItem 
                            {{ 
                                Header = "{0}", 
                                Command = new RelayCommand(_ => {1}.Show()) 
                            }});
            """;

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => node is ClassDeclarationSyntax,
                (ctx, _) => (ITypeSymbol)ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(provider.Collect(), Generate);
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
                    partial class ActionMenuButton
                    {
                        private class ActionMenu : ContextMenu
                        {
                            public ActionMenu()
                            {
                """);
            foreach (var symb in arr.Distinct<ITypeSymbol>(SymbolEqualityComparer.Default))
            {
                if (!symb.IsCommonClass()) continue;
                if (!symb.InheritedFrom(BASE)) continue;
                var attr = symb.GetAttributes().FirstOrDefault(a => a.AttributeClass.ToDisplayString() == ATTRIBUTE);
                if (attr is null) continue;
                builder.AppendLine(string.Format(TEMPLATE, attr?.ConstructorArguments[0].Value, symb.ToDisplayString()));
            }
            builder.AppendLine(
                """
                            }
                        }
                    }
                }
                """);
            ctx.AddSource("ActionsMenu.g.cs", builder.ToString());
        }
    }
}
