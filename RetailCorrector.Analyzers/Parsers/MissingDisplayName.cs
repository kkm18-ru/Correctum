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
        private const string Title = "Неявная регистрация парсера";
        private const string MessageFormat = "У класса {0} отсутствует атрибут [RegisterParser]";
        private const string Description =
            "Для добавления парсера в Release-сборку нужно ее явно зарегистрировать, атрибутом [RegisterParser].";
        private const string HelpUri = "https://gitlab.com/ornaras/retailcorrector/-/wikis/analyzers/parser#rc1000";

        private readonly static DiagnosticDescriptor Rule = new(
            DiagnosticIds.MissingDisplayNameParser,
            Title, MessageFormat, DiagnosticCategories.Parser,
            DiagnosticSeverity.Warning, true, Description, HelpUri);

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

            if (classDecl.BaseList == null)return;

            var semanticModel = context.SemanticModel;
            var classSymbol = semanticModel.GetDeclaredSymbol(classDecl);
            if (classSymbol == null || classSymbol.IsAbstract || classSymbol.IsStatic)
                return;

            var baseType = classSymbol.BaseType;
            if (!baseType.ToDisplayString().StartsWith("RetailCorrector.Parsers.ParserBase"))
                return;

            var hasMyRequiredAttribute = classSymbol.GetAttributes()
                .Any(attr => attr.AttributeClass?.ToDisplayString() == "RetailCorrector.Attributes.RegisterParserAttribute");

            if (!hasMyRequiredAttribute)
            {
                var diagnostic = Diagnostic.Create(
                    Rule,
                    classDecl.Identifier.GetLocation(),
                    classSymbol.Name,
                    baseType.Name
                );
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
