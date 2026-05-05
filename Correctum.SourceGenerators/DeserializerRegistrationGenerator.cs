using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

namespace Correctum.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class DeserializerRegistrationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource("DeserializerInfoAttribute.g.cs", """
            namespace Correctum.Attrubutes;

            public sealed class DeserializerInfoAttribute(
                string DisplayName) 
                : global::System.Attribute;

            """));

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(FilterSyntaxNode, Transform)
            .Where(t => t.@using)
            .Select((t, _) => t.Item1);

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) => GenerateCode(ctx, t.Left, t.Right));
    }

    private static bool FilterSyntaxNode(SyntaxNode node, CancellationToken cancellationToken)
    {
        if (node is not ClassDeclarationSyntax cds) return false;
        if (cds.AttributeLists is not { Count: > 0 } attrLists) return false;
        if (!attrLists.Any(list => list.Attributes.Any(a => a.ToString().Contains("DeserializerInfo"))))
            return false;
        if (cds.BaseList is not BaseListSyntax baseList) return false;
        if (!baseList.Types.Any(t => t.ToString().Contains("IDeserializer"))) return false;
        return true;
    }

    private static (ClassDeclarationSyntax, bool @using) Transform
        (GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var cds = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeList in cds.AttributeLists)
        {
            foreach (var attribute in attributeList.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol)
                    continue;

                if (attributeSymbol.ContainingType.ToDisplayString() != "Correctum.Attrubutes.DeserializerInfoAttribute")
                    continue;

                return (cds, true);
            }
        }

        return (cds, false);
    }

    private static void GenerateCode(SourceProductionContext context, Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> classDeclarations)
    {
        var builder = new StringBuilder();
        builder.AppendLine("""
            namespace Correctum.ViewModels.Windows
            {
                partial class PasteWindowViewModel
                {
                    public static partial global::System.Collections.Immutable.ImmutableArray<DeserializerDescriptor> Deserializers => [
            """);
        foreach (var cds in classDeclarations)
        {
            var model = compilation.GetSemanticModel(cds.SyntaxTree);
            if (model.GetDeclaredSymbol(cds) is not INamedTypeSymbol symbol)
                continue;
            var typePath = $"{symbol.ContainingNamespace.ToDisplayString()}.{cds.Identifier.Text}";
            ImmutableArray<TypedConstant> attrArgs = symbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "Correctum.Attrubutes.DeserializerInfoAttribute")?
                .ConstructorArguments ?? ImmutableArray<TypedConstant>.Empty;
            var displayName = string.Empty;
            if (attrArgs.Length == 1) displayName = (string?)attrArgs[0].Value ?? string.Empty;

            if (!symbol.AllInterfaces.Any(i => i.ConstructedFrom.ToDisplayString() == "Correctum.Deserializers.IDeserializer"))
                continue;
            builder.AppendLine(@$"            new DeserializerDescriptor(""{displayName}"", {typePath}.Deserialize),");
        }
        builder.AppendLine("""    
                    ];
                }
            }
            """);
        context.AddSource($"PasteWindowViewModel.Deserializers.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }
}
