using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RetailCorrector.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class RegisterParsersGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var sources = context.SyntaxProvider.CreateSyntaxProvider(
                (n, _) => n is ClassDeclarationSyntax,
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node))
                .Collect().Combine(context.GetConfigurationName());
            context.RegisterSourceOutput(sources, (ctx, v) => Register(ctx, v.Left, v.Right));
        }

        private static void Register(SourceProductionContext context, ImmutableArray<ISymbol> array, string conf)
        {
            var builder = new StringBuilder();
            builder.AppendLine(
                """
                using System.Windows.Controls;

                namespace RetailCorrector.UI
                {
                    internal partial class Parser
                    {
                        public readonly static KeyValuePair<string, Func<Control>>[] Parsers = [
                """);
            var hash = new HashSet<string>();
            foreach (var symb in array)
            {
                if (symb is not ITypeSymbol type || type.IsAbstract || type.IsStatic) continue;
                if (!type.BaseType?.ToDisplayString().StartsWith($"RetailCorrector.Parsers.ParserBase") ?? true)
                    continue;
                if (hash.Add(type.ToDisplayString()))
                {
                    string displayName = null;
                    if (type.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "RetailCorrector.Attributes.RegisterParserAttribute") is { } attr)
                        displayName = (string)attr.ConstructorArguments[0].Value!;
                    if(displayName is null)
                    {
                        if (conf.ToUpper() == "DEBUG")
                        {
                            var @class = type.ToDisplayString();
                            var _displayName = @class.Split('.');
                            builder.AppendLine($"            new(\"[DEBUG] {_displayName[_displayName.Length - 1].Replace("Presenter", "")}\", {@class}.ShowView),");
                        }
                    }else builder.AppendLine($"            new(\"{displayName}\", {type.ToDisplayString()}.ShowView),");
                }
            }
            builder.AppendLine($$"""
                                       ];
                                   }
                               }
                               """);

            context.AddSource("Parsers.g.cs", builder.ToString());
        }
    }
}
