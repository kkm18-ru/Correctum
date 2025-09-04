using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RetailCorrector.Analyzers.Parsers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvalidGenerateInfo : DiagnosticAnalyzer
    {
        private const string Title = "Некорректная генерация информации о парсинге";
        private const string MessageFormat = "Некорректно реализован метод GenerateInfo";
        private const string Description =
            "При генерации информации о парсинге нужно сохранять ее в поле parseInfo.";
        private const string HelpUri = "https://gitlab.com/ornaras/retailcorrector/-/wikis/analyzers-codes/rc1xxx#rc1001";

        private readonly static DiagnosticDescriptor Rule = new(
            DiagnosticIds.InvalidGenerateInfoParser,
            Title, MessageFormat, DiagnosticCategories.Parser,
            DiagnosticSeverity.Error, true, Description, HelpUri);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context) { }
    }
}
