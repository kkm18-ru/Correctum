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
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(AttributeGenerate);
            var provider = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => node is ClassDeclarationSyntax,
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(provider.Collect(), Generate);
        }

        private void AttributeGenerate(IncrementalGeneratorPostInitializationContext ctx)
        {
            var builder = new StringBuilder();
            builder.AppendLine(
                """
                namespace RetailCorrector;
                
                [AttributeUsage(AttributeTargets.Class)]
                internal class RegisterActionAttribute(string displayName) : Attribute;
                """);
            ctx.AddSource("RegisterActionAttribute.g.cs", builder.ToString());
        }

        private void Generate(SourceProductionContext ctx, ImmutableArray<ISymbol> arr)
        {
            const string TEMPLATE = "                Items.Add(new MenuItem {{ Header = \"{0}\", Command = new RelayCommand(_ => {1}.Show()) }});";

            var builder = new StringBuilder();
            var debug = new StringBuilder();
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
            debug.AppendLine("#if DEBUG");
            var hash = new HashSet<string>();
            foreach (var symb in arr)
            {
                if (symb is not ITypeSymbol type || type.IsStatic || type.IsAbstract) continue;
                if (!type.BaseType?.ToDisplayString().StartsWith("RetailCorrector.Actions.ActionBase") ?? true) continue;
                if (!hash.Add(type.ToDisplayString())) continue;
                var attr = type.GetAttributes().FirstOrDefault(a => a.AttributeClass.ToDisplayString() == "RetailCorrector.RegisterActionAttribute");
                if(attr is null)
                    debug.AppendLine(string.Format(TEMPLATE, type.Name, type.ToDisplayString()));
                else builder.AppendLine(string.Format(TEMPLATE, attr?.ConstructorArguments[0].Value, type.ToDisplayString()));
            }
            debug.AppendLine("#endif");
            builder.AppendLine(debug.ToString());
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
