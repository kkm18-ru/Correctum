using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace RetailCorrector.Analyzers.Actions
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MissingDisplayName : DiagnosticAnalyzer
    {
        private const string Title = "Неявная регистрация действия";
        private const string MessageFormat = "У класса {0} отсутствует атрибут [RegisterAction]";
        private const string Description =
            "Для добавления действия в Release-сборку нужно ее явно зарегистрировать, атрибутом [RegisterAction].";
        private const string HelpUri = "https://gitlab.com/ornaras/retailcorrector/-/wikis/analyzers/action#rc2000";

        private readonly static DiagnosticDescriptor Rule = new(
            DiagnosticIds.MissingDisplayNameAction,
            Title, MessageFormat, DiagnosticCategories.Action,
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
            if (!baseType.ToDisplayString().StartsWith("RetailCorrector.Actions.ActionBase"))
                return;

            var hasMyRequiredAttribute = classSymbol.GetAttributes()
                .Any(attr => attr.AttributeClass?.ToDisplayString() == "RetailCorrector.Attributes.RegisterActionAttribute");

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
