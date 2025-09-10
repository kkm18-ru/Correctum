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
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node))
                .Collect().Combine(context.GetConfigurationName());
            context.RegisterSourceOutput(provider, (ctx, v) => Generate(ctx, v.Left, v.Right));
        }

        private static void Generate(SourceProductionContext ctx, ImmutableArray<ISymbol> arr, string conf)
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
            var hash = new HashSet<string>();
            foreach (var symb in arr)
            {
                if (!symb.IsCommonClass(out var type)) continue;
                if (!type.InheritedFrom("RetailCorrector.Actions.ActionBase")) continue;
                if (!hash.Add(type.ToDisplayString())) continue;
                var attr = type.GetAttributes().FirstOrDefault(a => a.AttributeClass.ToDisplayString() == ATTRIBUTE);
                if(attr is null)
                {
                    if(conf.ToUpper() == "DEBUG")
                        builder.AppendLine(string.Format(TEMPLATE, type.Name, type.ToDisplayString()));
                }
                else builder.AppendLine(string.Format(TEMPLATE, attr?.ConstructorArguments[0].Value, type.ToDisplayString()));
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
