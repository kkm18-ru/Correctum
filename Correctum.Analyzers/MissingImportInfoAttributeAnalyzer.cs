using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Immutable;
using System.Composition;

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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingImportInfoAttributeCodeFixProvider)), Shared]
public sealed class MissingImportInfoAttributeCodeFixProvider : CodeFixProvider
{
    private const string Title = "Добавить атрибут ImportInfo";
    private const string UsingName = "Correctum.Attributes";
    private const string DefaultArgumentValue = "Источник";
    private const string AttributeName = "ImportInfo";

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(MissingImportInfoAttributeAnalyzer.DiagnosticId);

    public sealed override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var diagnostic = context.Diagnostics.First();
        var diagSpan = diagnostic.Location.SourceSpan;
        var declaration = root!.FindToken(diagSpan.Start).Parent!.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();
        context.RegisterCodeFix(
            CodeAction.Create(
                title: Title,
                createChangedDocument: c => AddAttributeAsync(context.Document, declaration, c),
                equivalenceKey: Title),
            diagnostic);
    }

    private static async Task<Document> AddAttributeAsync(Document document, TypeDeclarationSyntax declaration, CancellationToken c)
    {
        var editor = await DocumentEditor.CreateAsync(document, c).ConfigureAwait(false);
        var generator = editor.Generator;
            
        var valueAttributeArgument = generator.LiteralExpression(DefaultArgumentValue);
        var attributeArgument = generator.AttributeArgument(valueAttributeArgument);
        var attribute = generator.Attribute(AttributeName, attributeArgument);
        editor.AddAttribute(declaration, attribute);
        var changedDocument = editor.GetChangedDocument();

        var root = await changedDocument.GetSyntaxRootAsync(c).ConfigureAwait(false);
        if(root is CompilationUnitSyntax unit && 
            !unit.Usings.Any(u => u.Name.ToString() == UsingName))
        {
            var usingPath = SyntaxFactory.ParseName(UsingName);
            var @using = SyntaxFactory.UsingDirective(usingPath);
            changedDocument = changedDocument.WithSyntaxRoot(unit.AddUsings(@using));
        }

        return changedDocument;
    }
}
