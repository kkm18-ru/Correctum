using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using System.Collections.Immutable;
using System.Composition;

namespace Correctum.Analyzers;

[Shared, ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingImportInfoAttributeCodeFixProvider))]
public sealed class MissingImportInfoAttributeCodeFixProvider()
    : MissingModuleInfoAttributeCodeFixProvider(
        MissingModuleInfoAttributeAnalyzer.ImportDiagnosticId,
        title: "Добавить атрибут ImportInfoAttribute",
        attributeName: "ImportInfo",
        defaultArgumentValue: "Источник");

[Shared, ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingReportInfoAttributeCodeFixProvider))]
public sealed class MissingReportInfoAttributeCodeFixProvider()
    : MissingModuleInfoAttributeCodeFixProvider(
        MissingModuleInfoAttributeAnalyzer.ReportDiagnosticId,
        title: "Добавить атрибут ReportInfoAttribute",
        attributeName: "ReportInfo",
        defaultArgumentValue: "Отчет");

[Shared, ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(MissingDeserializerInfoAttributeCodeFixProvider))]
public sealed class MissingDeserializerInfoAttributeCodeFixProvider()
    : MissingModuleInfoAttributeCodeFixProvider(
        MissingModuleInfoAttributeAnalyzer.DeserializerDiagnosticId,
        title: "Добавить атрибут DeserializerInfoAttribute",
        attributeName: "DeserializerInfo",
        defaultArgumentValue: "Десериализатор");

public abstract class MissingModuleInfoAttributeCodeFixProvider
    (string diagnosticId, string title, string attributeName, string defaultArgumentValue)
    : CodeFixProvider
{
    private const string UsingName = "Correctum.Attributes";

    private readonly string diagnosticId = diagnosticId;
    private readonly string title = title;
    private readonly string attributeName = attributeName;
    private readonly string defaultArgumentValue = defaultArgumentValue;

    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(diagnosticId);

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
                title: title,
                createChangedDocument: c => AddAttributeAsync(context.Document, declaration, c),
                equivalenceKey: title),
            diagnostic);
    }

    private async Task<Document> AddAttributeAsync
        (Document document, TypeDeclarationSyntax declaration, CancellationToken c)
    {
        var editor = await DocumentEditor.CreateAsync(document, c).ConfigureAwait(false);
        var generator = editor.Generator;

        var valueAttributeArgument = generator.LiteralExpression(defaultArgumentValue);
        var attributeArgument = generator.AttributeArgument(valueAttributeArgument);
        var attribute = generator.Attribute(attributeName, attributeArgument);
        editor.AddAttribute(declaration, attribute);
        var changedDocument = editor.GetChangedDocument();

        var root = await changedDocument.GetSyntaxRootAsync(c).ConfigureAwait(false);
        if (root is CompilationUnitSyntax unit &&
            !unit.Usings.Any(u => u.Name.ToString() == UsingName))
        {
            var usingPath = SyntaxFactory.ParseName(UsingName);
            var @using = SyntaxFactory.UsingDirective(usingPath);
            changedDocument = changedDocument.WithSyntaxRoot(unit.AddUsings(@using));
        }

        return changedDocument;
    }
}
