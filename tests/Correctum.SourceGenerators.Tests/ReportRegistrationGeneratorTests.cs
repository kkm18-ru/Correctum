using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Xunit;

namespace Correctum.SourceGenerators.Tests;

public class ReportRegistrationGeneratorTests
{
    private const string SourceText = 
        $$"""
        namespace Correctum.Reports;

        public interface IReport { }
                
        [Correctum.Attrubutes.ReportInfo("Тестовый отчет")]
        public partial class TestReport : IReport { }
        
        """;

    private const string ExpectedGeneratedText = 
        $$"""
        namespace Correctum.ViewModels.Controls
        {
            partial class MainMenuViewModel
            {
                public static partial global::System.Collections.Immutable.ImmutableArray<ReportDescriptor> Reports => [
                    new ReportDescriptor("Тестовый отчет", Correctum.Reports.TestReport.GenerateReport),
                ];
            }
        }

        """;

    [Fact]
    public void GenerateRegistrationReports()
    {
        var generator = new ReportRegistrationGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        var compilation = CSharpCompilation.Create(
            nameof(DeserializerRegistrationGeneratorTests),
            [CSharpSyntaxTree.ParseText(SourceText)],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var runResult = driver.RunGenerators(compilation).GetRunResult();
        var generated = runResult.GeneratedTrees.Single
            (t => t.FilePath.EndsWith("MainMenuViewModel.Reports.g.cs"));
        Assert.Equal(ExpectedGeneratedText, generated.GetText().ToString(),
            ignoreLineEndingDifferences: true);
    }
}
