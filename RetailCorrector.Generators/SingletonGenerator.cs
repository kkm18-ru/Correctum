using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace RetailCorrector.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class SingletonGenerator : IIncrementalGenerator
    {
        const string ATTRIBUTE = "RetailCorrector.Attributes.SingletonAttribute";
        static HashSet<string> hash = new();

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classes = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => node is ClassDeclarationSyntax,
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(classes, Generate);
        }

        private static void Generate(SourceProductionContext context, ISymbol symbol)
        {
            var type = (ITypeSymbol)symbol;
            if (type.GetAttributes().All(a => a.AttributeClass.ToDisplayString() != ATTRIBUTE)) return;
            if (!hash.Add(type.ToDisplayString())) return;
            context.AddSource($"{type.Name}.Singleton.g.cs",
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