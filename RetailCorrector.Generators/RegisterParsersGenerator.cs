using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RetailCorrector.Generators
{
    [Generator]
    internal class RegisterParsersGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var sources = context.SyntaxProvider.CreateSyntaxProvider(
                (n, _) => n is ClassDeclarationSyntax syntax,
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(sources.Collect(), Execute);
        }

        private static void Execute(SourceProductionContext ctx, ImmutableArray<ISymbol> symbols)
        {
            var builder = new StringBuilder();
            builder.AppendLine("""
                               using System.Windows.Controls;

                               namespace RetailCorrector.Parsers
                               {
                                   internal partial class Parser
                                   {
                                       public readonly static KeyValuePair<string, Func<Control>>[] Parsers = [
                               """);
            var hash = new HashSet<string>();
            foreach(var symb in symbols)
            {
                if (symb is not ITypeSymbol type) continue;
                if (type.Interfaces.All(i => !i.ToDisplayString().StartsWith("RetailCorrector.Parsers.IParserPresenter"))) continue;
                if (!hash.Contains(type.ToDisplayString()))
                {
                    builder.AppendLine($"            new({type.ToDisplayString()}.DisplayName, {type.ToDisplayString()}.ShowView),");
                    hash.Add(type.ToDisplayString());
                }
            }
            builder.AppendLine("""
                                       ];
                                   }
                               }
                               """);

            ctx.AddSource("Parsers.g.cs", builder.ToString());
        }
    }
}
