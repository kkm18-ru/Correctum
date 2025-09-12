using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Linq;

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
            array = [..array.Where(type => type.GetAttributes()
                .Any(ad => ad.AttributeClass?.ToDisplayString() == ATTRIBUTE))];
            foreach (var type in array.Distinct<ITypeSymbol>(SymbolEqualityComparer.Default))
            {
                context.AddSource($"{type.Name}_Singleton.g.cs",
                    $$"""
                      namespace {{type.ContainingNamespace.ToDisplayString()}};

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
                      }
                      """);
            }
        }
    }
}