using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RetailCorrector.Generators
{
    [Generator]
    internal class RegisterActionsGenerator : IIncrementalGenerator
    {
        const string ATTRIBUTE = "RetailCorrector.Attributes.RegisterActionAttribute";
        const string TEMPLATE = "Items.Add(new MenuItem {{ Header = \"{0}\", Command = new RelayCommand(_ => {1}.Show()) }});";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => node is ClassDeclarationSyntax,
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(provider.Collect(), Generate);
        }

        private static void Generate(SourceProductionContext ctx, ImmutableArray<ISymbol> arr)
        {
            var builder = new StringBuilder();
            builder.AppendLine(
                """
                using RetailCorrector.Utils;
                using System.Windows.Controls;

                namespace RetailCorrector.UI.Controls
                {
                    internal partial class ActionMenuButton
                    {
                        private class ActionMenu : ContextMenu
                        {
                            public ActionMenu()
                            {
                """);
            var debug = new StringBuilder();
            debug.AppendLine("#if DEBUG");
            var hash = new HashSet<string>();
            foreach (var symb in arr)
            {
                if (symb is not ITypeSymbol type || type.IsStatic || type.IsAbstract) continue;
                if (!type.BaseType?.ToDisplayString().StartsWith("RetailCorrector.Actions.ActionBase") ?? true) continue;
                if (!hash.Add(type.ToDisplayString())) continue;
                var attr = type.GetAttributes().FirstOrDefault(a => a.AttributeClass.ToDisplayString() == ATTRIBUTE);
                if(attr is null)
                    debug.AppendLine(string.Format(TEMPLATE, type.Name, type.ToDisplayString()));
                else builder.AppendLine(string.Format(TEMPLATE, attr?.ConstructorArguments[0].Value, type.ToDisplayString()));
            }
            builder.AppendLine(debug.ToString());
            builder.AppendLine(
                """
                #endif
                            }
                        }
                    }
                }
                """);
            ctx.AddSource("ActionsMenu.g.cs", builder.ToString());
        }
    }
}
