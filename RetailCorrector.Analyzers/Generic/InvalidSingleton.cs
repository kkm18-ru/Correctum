using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RetailCorrector.Analyzers.Generic;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class InvalidSingleton : DiagnosticAnalyzer
{
    private const string Title = "Singleton-класс должен иметь только частный конструктор без параметров";
    private const string MessageFormat = "Класс {0} должен иметь только один конструктор (частный и без параметров)";
    private const string Description = 
        "Все публичные или конструкторы с параметрами нарушают принцип Singleton и " +
        "могут позволить создать более одного экземпляра класса.";
    private const string HelpUri = "https://gitlab.com/ornaras/retailcorrector/-/wikis/analyzers/0-generic#rc0001";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticIds.InvalidSingleton,
        Title, MessageFormat, DiagnosticCategories.Generic,
        DiagnosticSeverity.Error, true, Description, HelpUri);
    
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private static bool HasSingletonAttribute(ClassDeclarationSyntax cds) =>
        cds.AttributeLists.SelectMany(l => l.Attributes)
            .Any(a => a.Name is IdentifierNameSyntax ins && ins.Identifier.Text.StartsWith("Singleton"));

    private static bool HasOnlyOneConstructor(ClassDeclarationSyntax cds) =>
        cds.Members.Count(m => m is ConstructorDeclarationSyntax) == 1;

    private static bool ValidConstructor(ConstructorDeclarationSyntax cds) =>
        cds.Modifiers.Any(m => m.Text == "private") && cds.ParameterList.Parameters.Count == 0;
    
    private static ConstructorDeclarationSyntax GetConstructorSyntax(ClassDeclarationSyntax cds) =>
        (ConstructorDeclarationSyntax)cds.Members.First(m => m is ConstructorDeclarationSyntax);

    private static bool ValidSingleton(ClassDeclarationSyntax cds) =>
        HasOnlyOneConstructor(cds) && ValidConstructor(GetConstructorSyntax(cds));
    
    private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var cds = (ClassDeclarationSyntax)context.Node;
        if (!HasSingletonAttribute(cds) || ValidSingleton(cds)) return;
        var semanticModel = context.SemanticModel;
        var classSymbol = ModelExtensions.GetDeclaredSymbol(semanticModel, cds);
        var diagnostic = Diagnostic.Create(Rule, cds.Identifier.GetLocation(), classSymbol!.Name);
        context.ReportDiagnostic(diagnostic);
    }
}