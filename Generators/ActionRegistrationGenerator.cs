namespace Correctum.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class ActionRegistrationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(FilterSyntaxNode, Transform)
            .Where(t => t.@using)
            .Select((t, _) => t.Item1);

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()), 
            (ctx, t) => RegisterReports(ctx, t.Left, t.Right));
    }

    private static bool FilterSyntaxNode(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax cds && cds.BaseList is { };
    }

    private static (ClassDeclarationSyntax, bool @using) Transform
        (GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var cds = (ClassDeclarationSyntax)context.Node;

        foreach (var baseType in cds.BaseList?.Types ?? [])
        {
            if(baseType.Type.ToString().Contains("IReceiptAction"))
                return (cds, true);
        }

        return (cds, false);
    }

    private static void RegisterReports(SourceProductionContext context, Compilation compilation,
        ImmutableArray<ClassDeclarationSyntax> classDeclarations)
    {
        var builder = new StringBuilder();
        builder.AppendLine("""
            namespace Correctum.ViewModels.Controls
            {
                partial class MainMenuViewModel
                {
                    public static partial global::System.Collections.Immutable.ImmutableArray<ActionDescriptor> Actions => [
            """);
        foreach (var cds in classDeclarations)
        {
            var model = compilation.GetSemanticModel(cds.SyntaxTree);
            if (model.GetDeclaredSymbol(cds) is not INamedTypeSymbol symbol)
                continue;
            var typePath = $"{symbol.ContainingNamespace.ToDisplayString()}.{cds.Identifier.Text}";

            if (!symbol.AllInterfaces.Any(i => i.ConstructedFrom.ToDisplayString().Contains("Correctum.Abstractions.IReceiptAction")))
                continue;
            builder.AppendLine(@$"            new ActionDescriptor({typePath}.Name, typeof({typePath})),");
        }
        builder.AppendLine("""    
                    ];
                }
            }
            """);
        context.AddSource($"MainMenuViewModel.Actions.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }
}
