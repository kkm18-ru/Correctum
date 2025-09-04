using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RetailCorrector.Analyzers.Parsers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class InvalidGenerateInfo : DiagnosticAnalyzer
    {
        private const string Title = "Некорректная генерация информации о парсинге";
        private const string MessageFormat = "Некорректно реализован метод GenerateInfo";
        private const string Description = "При генерации информации о парсинге нужно сохранять ее в поле parseInfo.";
        private const string HelpUri = "https://gitlab.com/ornaras/retailcorrector/-/wikis/analyzers/parser#rc1001";

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

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not MethodDeclarationSyntax mds) return;
            if (mds.Identifier.Text != "GenerateInfo") return;
            if (mds.Modifiers.Count == 0 || mds.Modifiers.Any(m => m.Text == "abstract")) return;
            if (mds.ExpressionBody is not null)
            {
                if (mds.ExpressionBody.Expression is AssignmentExpressionSyntax aes && 
                    aes.OperatorToken.Text == "=" && aes.Left is IdentifierNameSyntax ins && 
                    ins.Identifier.Text == "parseInfo") return;
            }
            else return;
            context.ReportDiagnostic(Diagnostic.Create(Rule, mds.Identifier.GetLocation()));
        }
    }
}
