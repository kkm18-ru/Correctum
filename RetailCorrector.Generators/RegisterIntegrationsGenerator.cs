#nullable enable
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Item = (string ClassName, string? DisplayName);

namespace RetailCorrector.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class RegisterIntegrationsGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(GenerateAttribute);
            var sources = context.SyntaxProvider.CreateSyntaxProvider(
                (n, _) => n is ClassDeclarationSyntax syntax,
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(sources.Collect(), Register);
        }

        private void Register(SourceProductionContext context, ImmutableArray<ISymbol?> array)
        {
            var parsers = new List<Item>();

            var hash = new HashSet<string>();
            foreach (var symb in array)
            {
                if (symb is not ITypeSymbol type || type.IsAbstract || type.IsStatic) continue;
                if (!CheckBaseType(type, "Parser"))
                    continue;
                if (hash.Add(type.ToDisplayString()))
                {
                    string? displayName = null;
                    if (type.GetAttributes().FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "RetailCorrector.RegisterIntegrationAttribute") is { } attr)
                        displayName = (string)attr.ConstructorArguments[0].Value!;
                    parsers.Add((type.ToDisplayString(), displayName));
                }
            }

            RegisterParsers(context, [.. parsers]);
        }

        private bool CheckBaseType(ITypeSymbol symbol, string @class) =>
            symbol.BaseType?.ToDisplayString().StartsWith($"RetailCorrector.{@class}s.{@class}Presenter") ?? false;

        private void RegisterParsers(SourceProductionContext context, ImmutableArray<Item> array)
        {

            var builder = new StringBuilder();
            builder.AppendLine("""
                               using System.Windows.Controls;

                               namespace RetailCorrector.Parsers
                               {
                                   internal partial class Parser
                                   {
                                       public readonly static KeyValuePair<string, Func<Control>>[] Parsers = [
                               #if DEBUG
                               """);
            foreach (var (@class, _) in array.Where(i => i.DisplayName is null))
            {
                var displayName = @class.Split('.');
                builder.AppendLine($"            new(\"[DEBUG] {displayName[displayName.Length - 1].Replace("Presenter", "")}\", {@class}.ShowView),");
            }
            builder.AppendLine("#endif");
            foreach (var (@class, display) in array.Where(i => i.DisplayName is not null))
                builder.AppendLine($"            new(\"{display}\", {@class}.ShowView),");
            builder.AppendLine("""
                                       ];
                                   }
                               }
                               """);

            context.AddSource("Parsers.g.cs", builder.ToString());
        }

        private void GenerateAttribute(IncrementalGeneratorPostInitializationContext ctx) => 
            ctx.AddSource("RegisterIntegrationAttribute.g.cs",
                """
                namespace RetailCorrector;

                [AttributeUsage(AttributeTargets.Class)]
                internal class RegisterIntegrationAttribute(string displayName) : Attribute;
                """);
    }
}
