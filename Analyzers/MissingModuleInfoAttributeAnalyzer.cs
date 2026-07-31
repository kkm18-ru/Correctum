using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace Correctum.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MissingModuleInfoAttributeAnalyzer : DiagnosticAnalyzer
{
    internal const string ImportDiagnosticId = "CORR1000";
    private const string ImportTitle = "У импортера отсутствует атрибут ImportInfoAttribute";
    private const string ImportMessageFormat = "У типа '{0}' отсутствует атрибут ImportInfoAttribute";
    private const string ImportDescription = "Чтобы импортер был зарегистрирован и отображался в меню, нужно добавить к нему атрибут Correctum.Attributes.ImportInfoAttribute.";
    private const string ImportAttributeFullPath = "Correctum.Attributes.ImportInfoAttribute";
    private const string ImportInterfaceFullPath = "Correctum.Abstractions.IImport";

    internal const string DeserializerDiagnosticId = "CORR1001";
    private const string DeserializerTitle = "У десериализатора отсутствует атрибут DeserializerInfoAttribute";
    private const string DeserializerMessageFormat = "У типа '{0}' отсутствует атрибут DeserializerInfoAttribute";
    private const string DeserializerDescription = "Чтобы десериализатор был зарегистрирован и отображался в меню, нужно добавить к нему атрибут Correctum.Attributes.DeserializerInfoAttribute.";
    private const string DeserializerAttributeFullPath = "Correctum.Attributes.DeserializerInfoAttribute";
    private const string DeserializerInterfaceFullPath = "Correctum.Abstractions.IDeserializer";

    internal const string ReportDiagnosticId = "CORR1002";
    private const string ReportTitle = "У отчета отсутствует атрибут ReportInfoAttribute";
    private const string ReportMessageFormat = "У типа '{0}' отсутствует атрибут ReportInfoAttribute";
    private const string ReportDescription = "Чтобы отчет был зарегистрирован и отображался в меню, нужно добавить к нему атрибут Correctum.Attributes.ReportInfoAttribute.";
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

        if (symbol.AllInterfaces.Any(i => i.ToDisplayString() == ImportInterfaceFullPath) && 
            !symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == ImportAttributeFullPath))
        {
            var diagnostic = Diagnostic.Create(ImportRule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        if (symbol.AllInterfaces.Any(i => i.ToDisplayString() == DeserializerInterfaceFullPath) && 
            !symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == DeserializerAttributeFullPath))
        {
            var diagnostic = Diagnostic.Create(DeserializerRule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        if (symbol.AllInterfaces.Any(i => i.ToDisplayString() == ReportInterfaceFullPath) && 
            !symbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == ReportAttributeFullPath))
        {
            var diagnostic = Diagnostic.Create(ReportRule, symbol.Locations[0], symbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}
