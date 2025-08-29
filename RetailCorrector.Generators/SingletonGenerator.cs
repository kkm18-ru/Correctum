using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace RetailCorrector.Generators
{
    [Generator(LanguageNames.CSharp)]
    internal class SingletonGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(CreateAttribute);
            var classes = context.SyntaxProvider.CreateSyntaxProvider(
                (node, _) => node is ClassDeclarationSyntax,
                (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol(ctx.Node));
            context.RegisterSourceOutput(classes, Generate);
        }

        private void Generate(SourceProductionContext context, ISymbol symbol)
        {
            var type = (ITypeSymbol)symbol;
            if (type.GetAttributes().All(a => a.AttributeClass.ToDisplayString() != "RetailCorrector.Attributes.SingletonAttribute")) return;
            context.AddSource($"{type.Name}.Singleton.g.cs",
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
                    }
                }
                """);
        }

        private static void CreateAttribute(IncrementalGeneratorPostInitializationContext ctx) =>
            ctx.AddSource("SingletonAttribute.g.cs",
                """
                using System;

                namespace RetailCorrector.Attributes
                {
                    [AttributeUsage(AttributeTargets.Class)]
                    internal class SingletonAttribute : Attribute { }
                }
                """);
    }
}