namespace Correctum.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class ReportRegistrationGenerator : IIncrementalGenerator
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
        if (node is not ClassDeclarationSyntax cds) return false;
        if (cds.AttributeLists is not { Count: > 0 } attrLists) return false;
        if (!attrLists.Any(list => list.Attributes.Any(a => a.ToString().Contains("ReportInfo"))))
            return false;
        if (cds.BaseList is not BaseListSyntax baseList) return false;
        if (!baseList.Types.Any(t => t.ToString().Contains("IReport"))) return false;
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

                if (attributeSymbol.ContainingType.ToDisplayString() != "Correctum.Attributes.ReportInfoAttribute")
                    continue;

                return (cds, true);
            }
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
                    public static partial global::System.Collections.Immutable.ImmutableArray<ReportDescriptor> Reports => [
            """);
        foreach (var cds in classDeclarations)
        {
            var model = compilation.GetSemanticModel(cds.SyntaxTree);
            if (model.GetDeclaredSymbol(cds) is not INamedTypeSymbol symbol)
                continue;
            var typePath = $"{symbol.ContainingNamespace.ToDisplayString()}.{cds.Identifier.Text}";
            ImmutableArray<TypedConstant> attrArgs = symbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "Correctum.Attributes.ReportInfoAttribute")?
                .ConstructorArguments ?? ImmutableArray<TypedConstant>.Empty;
            var displayName = string.Empty;
            if (attrArgs.Length == 1) displayName = (string?)attrArgs[0].Value ?? string.Empty;

            if (!symbol.AllInterfaces.Any(i => i.ConstructedFrom.ToDisplayString() == "Correctum.Abstractions.IReport"))
                continue;
            builder.AppendLine(@$"            new ReportDescriptor(""{displayName}"", {typePath}.GenerateReport),");
        }
        builder.AppendLine("""    
                    ];
                }
            }
            """);
        context.AddSource($"MainMenuViewModel.Reports.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
    }
}
