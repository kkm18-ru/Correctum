using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace RetailCorrector.Analyzers.Parsers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingDisplayName : DiagnosticAnalyzer
    {
        private const string Title = "Парсером невозможно воспользоваться";
        private const string MessageFormat = "У класса {0} отсутствует атрибут [RegisterParser]";
        private const string Description = "Все классы-парсеры должны быть помечены атрибутом [RegisterParser].";
        private const string HelpUri = "https://gitlab.com/ornaras/retailcorrector/-/wikis/analyzers/1-parser#rc1000";
        private const string BasePath = "RetailCorrector.Parsers.ParserBase";
        private const string AttributePath = "RetailCorrector.Attributes.RegisterParserAttribute";

        private static readonly DiagnosticDescriptor Rule = new(
            DiagnosticIds.MissingDisplayNameParser,
            Title, MessageFormat, DiagnosticCategories.Parser,
            DiagnosticSeverity.Error, true, Description, HelpUri);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDecl = (ClassDeclarationSyntax)context.Node; 
            var semanticModel = context.SemanticModel;
            var symbol =  semanticModel.GetDeclaredSymbol(classDecl);
            if (!IsParser(symbol) || HasAttribute(symbol)) return;
            var diagnostic = Diagnostic.Create(Rule, classDecl.Identifier.GetLocation(), symbol!.Name);
            context.ReportDiagnostic(diagnostic);
        }

        private static bool IsCommonClass(INamedTypeSymbol classSymbol) =>
            classSymbol is not null && !classSymbol.IsAbstract && !classSymbol.IsStatic;

        private static bool IsParser(INamedTypeSymbol classSymbol) => IsCommonClass(classSymbol) &&
            (classSymbol.BaseType?.ToDisplayString().StartsWith(BasePath) ?? false);

        private static bool HasAttribute(INamedTypeSymbol classSymbol) =>
            classSymbol.GetAttributes().Any(IsNeedAttribute);

        private static bool IsNeedAttribute(AttributeData attr) =>
            attr.AttributeClass?.ToDisplayString() == AttributePath;
    }
}
