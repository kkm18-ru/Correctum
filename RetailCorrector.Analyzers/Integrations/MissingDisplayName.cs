using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RetailCorrector.Generators;
using System.Collections.Immutable;
using System.Linq;

namespace RetailCorrector.Analyzers.Integrations
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingDisplayName : DiagnosticAnalyzer
    {
        private const string Title = "Неявная регистрация интеграции";
        private const string MessageFormat = "У класса {0} отсутствует атрибут [RegisterIntegration]";
        private const string Description = 
            "Для добавления интеграции в Release-сборку нужно ее явно зарегистрировать, атрибутом [RegisterIntegration]";

        private readonly static DiagnosticDescriptor Rule = new(
            DiagnosticIds.MissingDisplayNameIntegration,
            Title, MessageFormat, DiagnosticCategories.Intergration,
            DiagnosticSeverity.Warning, true, Description);

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
            if (!baseType.ToDisplayString().StartsWith("RetailCorrector.Fiscals.FiscalBase") &&
                !baseType.ToDisplayString().StartsWith("RetailCorrector.Parsers.ParserBase"))
                return;

            var hasMyRequiredAttribute = classSymbol.GetAttributes()
                .Any(attr => attr.AttributeClass?.ToDisplayString() == "RetailCorrector.RegisterIntegrationAttribute");

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
