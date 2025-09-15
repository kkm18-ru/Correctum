using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace RetailCorrector.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class SingletonGenerator : IIncrementalGenerator
    {
        const string ATTRIBUTE = "RetailCorrector.Attributes.SingletonAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classes = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => node is ClassDeclarationSyntax,
                (gsc, _) => (ITypeSymbol)gsc.SemanticModel.GetDeclaredSymbol(gsc.Node)!);
            context.RegisterSourceOutput(classes.Collect(), Generate);
        }

        private static void Generate(SourceProductionContext context, ImmutableArray<ITypeSymbol> array)
        {
            var builder = new StringBuilder();
            array = [..array.Where(type => type.GetAttributes()
                .Any(ad => ad.AttributeClass?.ToDisplayString() == ATTRIBUTE))];
            foreach (var type in array.Distinct<ITypeSymbol>(SymbolEqualityComparer.Default))
            {
                builder.AppendLine(
                    $$"""
                      namespace {{type.ContainingNamespace.ToDisplayString()}}
                      {
                          {{type.GetAccessModifier()}} partial class {{type.Name}}
                          {
                              public static {{type.Name}} Instance
                              {
                                  get
                                  {
                                      if(_instance is null) 
                                          _instance = new {{type.Name}}();
                                      return _instance;
                                  }
                              }
                              private static {{type.Name}} _instance;

                      """);
                if (type is INamedTypeSymbol @class && NeedAutoConstructor(@class))
                    builder.AppendLine($"        private {type.Name}() {{ }}");
                builder.AppendLine(
                    $$"""
                          }
                      }

                      """);
            }
            context.AddSource("Singletons.g.cs", builder.ToString());
        }

        private static bool NeedAutoConstructor(INamedTypeSymbol type) =>
            type.InstanceConstructors.All(c => c.DeclaredAccessibility != Accessibility.Private || c.Parameters.Length > 0);
    }
}