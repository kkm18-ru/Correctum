using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Correctum.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingImportInfoAttributeAnalyzer : DiagnosticAnalyzer
{
    internal const string DiagnosticId = "CORR1000";
    private const string Title = "У импорт-модуля отсутствует атрибут ImportInfo";
    private const string MessageFormat = "У типа '{0}' отсутствует атрибут ImportInfo";
    private const string Description = "Чтобы импорт-модуль был зарегистрирован и отображался в меню, нужно использовать атрибут Correctum.Attributes.ImportInfoAttribute.";
    private const string Category = "Attributes";
    private const string AttributeFullPath = "Correctum.Attributes.ImportInfoAttribute";
    private const string InterfaceFullPath = "Correctum.Abstractions.IImport";

    private static readonly DiagnosticDescriptor Rule = 
        new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;
        if (symbol.AllInterfaces.Any(i => i.ToDisplayString() == InterfaceFullPath) && !symbol.IsAbstract &&
            !symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == AttributeFullPath))
        {
            var diagnostic = Diagnostic.Create(Rule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
