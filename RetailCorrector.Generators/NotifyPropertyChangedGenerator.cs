using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace RetailCorrector.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class NotifyPropertyChangedGenerator : IIncrementalGenerator
    {
        private const string AttributePath = "RetailCorrector.Attributes.NotifyChangedAttribute";

        private static bool Predicate(SyntaxNode node, CancellationToken token) =>
            node is PropertyDeclarationSyntax p && p.AttributeLists.Count > 0;

        private static IPropertySymbol Transform(GeneratorSyntaxContext ctx, CancellationToken token)
        {
            var prop = (PropertyDeclarationSyntax)ctx.Node;
            var model = ctx.SemanticModel;
            var symbol = model.GetDeclaredSymbol(prop);
            if (symbol == null) return default;
            var hasAttr = symbol.GetAttributes().Any(ad => ad.AttributeClass?.Name.Contains("NotifyChangedAttribute") ?? false);
            if (!hasAttr) return default;
            return symbol;
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var properties = context.SyntaxProvider
                .CreateSyntaxProvider(Predicate, Transform)
                .Where(symbol => symbol != null).Collect();
            context.RegisterSourceOutput(properties, Generate);
        }

        private static void Generate(SourceProductionContext context, ImmutableArray<IPropertySymbol> props)
        {
            if (props.IsDefaultOrEmpty) return;

            var classes = props.Where(p => p != null)
                .GroupBy(p => p!.ContainingType, SymbolEqualityComparer.Default);

            var builder = new StringBuilder();
            builder.AppendLine($$"""
                    using System.ComponentModel;
                    using System.Runtime.CompilerServices;
                    """);
            foreach (var group in classes)
            {
                var classSymbol = group.Key!;

                builder.AppendLine($$"""

                    namespace {{classSymbol.ContainingNamespace.ToDisplayString()}}
                    {
                        partial class {{classSymbol.Name}} : INotifyPropertyChanged
                        {
                    """);

                foreach (var prop in group)
                    builder.AppendLine(Property(prop));

                builder.AppendLine("""                                    
                            public event PropertyChangedEventHandler? PropertyChanged;
                            private void OnPropertyChanged([CallerMemberName] string propName = "") =>
                                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
                        }
                    }
                    """);
            }
            context.AddSource("NotifyChanged.g.cs", builder.ToString());
        }

        private static string GetDefaultValue(string typeName) => typeName switch
        {
            "string" => "\"\"",
            "System.DateTime" => "System.DateTime.Today",
            _ => "default!"
        };

        private static string Property(IPropertySymbol prop)
        {
            var typeName = prop!.Type.ToDisplayString();
            var propName = prop.Name;
            var fieldName = $"_{propName.ToLower()}";

            var param = prop.GetAttributes().First(a => a.AttributeClass.ToDisplayString().StartsWith(AttributePath)).ConstructorArguments[0];
            string defaultValue = param.IsNull ? GetDefaultValue(typeName) : param.ToCSharpString();

            return $$"""
                    private {{typeName}} {{fieldName}} = {{defaultValue}};
                    public partial {{typeName}} {{propName}}
                    {
                        get => {{fieldName}};
                        set
                        {
                            if ({{fieldName}} == value) return;
                            {{fieldName}} = value;
                            OnPropertyChanged();
                        }
                    }

            """;
        }
    }
}
