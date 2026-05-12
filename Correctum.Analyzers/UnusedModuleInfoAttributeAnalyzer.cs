using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Correctum.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnusedModuleInfoAttributeAnalyzer : DiagnosticAnalyzer
{
    internal const string ImportDiagnosticId = "CORR1003";
    private const string ImportTitle = "Лишний атрибут ImportInfoAttribute";
    private const string ImportMessageFormat = "У типа '{0}' присутствует атрибут ImportInfoAttribute";
    private const string ImportDescription = "Атрибут 'Correctum.Attributes.ImportInfoAttribute' используется только в импортерах.";
    private const string ImportAttributeFullPath = "Correctum.Attributes.ImportInfoAttribute";
    private const string ImportInterfaceFullPath = "Correctum.Abstractions.IImport";

    internal const string DeserializerDiagnosticId = "CORR1004";
    private const string DeserializerTitle = "Лишний атрибут DeserializerInfoAttribute";
    private const string DeserializerMessageFormat = "У типа '{0}' присутствует атрибут DeserializerInfoAttribute";
    private const string DeserializerDescription = "Атрибут 'Correctum.Attributes.DeserializerInfoAttribute' используется только в десериализаторах.";
    private const string DeserializerAttributeFullPath = "Correctum.Attributes.DeserializerInfoAttribute";
    private const string DeserializerInterfaceFullPath = "Correctum.Abstractions.IDeserializer";

    internal const string ReportDiagnosticId = "CORR1005";
    private const string ReportTitle = "Лишний атрибут ReportInfoAttribute";
    private const string ReportMessageFormat = "У типа '{0}' присутствует атрибут ReportInfoAttribute";
    private const string ReportDescription = "Атрибут 'Correctum.Attributes.ReportInfoAttribute' используется только в отчетах.";
    private const string ReportAttributeFullPath = "Correctum.Attributes.ReportInfoAttribute";
    private const string ReportInterfaceFullPath = "Correctum.Abstractions.IReport";

    private const string Category = "Attributes";
    private const DiagnosticSeverity Severity = DiagnosticSeverity.Warning;

    private static readonly DiagnosticDescriptor ImportRule =
        new(ImportDiagnosticId, ImportTitle, ImportMessageFormat, Category, Severity, true, ImportDescription);

    private static readonly DiagnosticDescriptor DeserializerRule =
        new(DeserializerDiagnosticId, DeserializerTitle, DeserializerMessageFormat, Category, Severity, true, DeserializerDescription);

    private static readonly DiagnosticDescriptor ReportRule =
        new(ReportDiagnosticId, ReportTitle, ReportMessageFormat, Category, Severity, true, ReportDescription);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DeserializerRule, ImportRule, ReportRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(Analyze, SymbolKind.NamedType);
    }

    private static void Analyze(SymbolAnalysisContext context)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;
        if (symbol.IsAbstract) return;

        if (symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == ImportAttributeFullPath) &&
            !symbol.AllInterfaces.Any(i => i.ToDisplayString() == ImportInterfaceFullPath))
        {
            var diagnostic = Diagnostic.Create(ImportRule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        if (symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == DeserializerAttributeFullPath) &&
            !symbol.AllInterfaces.Any(i => i.ToDisplayString() == DeserializerInterfaceFullPath))
        {
            var diagnostic = Diagnostic.Create(DeserializerRule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        if (symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == ReportAttributeFullPath) &&
            !symbol.AllInterfaces.Any(i => i.ToDisplayString() == ReportInterfaceFullPath))
        {
            var diagnostic = Diagnostic.Create(ReportRule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
