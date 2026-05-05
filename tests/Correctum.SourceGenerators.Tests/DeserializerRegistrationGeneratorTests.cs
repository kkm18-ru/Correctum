using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Xunit;

namespace Correctum.SourceGenerators.Tests;

public class DeserializerRegistrationGeneratorTests
{
    private const string SourceText = 
        $$"""
        namespace Correctum.Deserializers;

        public interface IDeserializer { }
                
        [Correctum.Attrubutes.DeserializerInfo("Тестовый десериализатор")]
        public partial class TestDeserializer : IDeserializer { }
        
        """;

    private const string ExpectedGeneratedText = 
        $$"""
        namespace Correctum.ViewModels.Windows
        {
            partial class PasteWindowViewModel
            {
                public static partial global::System.Collections.Immutable.ImmutableArray<DeserializerDescriptor> Deserializers => [
                    new DeserializerDescriptor("Тестовый десериализатор", Correctum.Deserializers.TestDeserializer.Deserialize),
                ];
            }
        }

        """;

    [Fact]
    public void GenerateRegistrationDeserializers()
    {
        var generator = new DeserializerRegistrationGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        var compilation = CSharpCompilation.Create(
            nameof(DeserializerRegistrationGeneratorTests),
            [CSharpSyntaxTree.ParseText(SourceText)],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)]);
        var runResult = driver.RunGenerators(compilation).GetRunResult();
        var generated = runResult.GeneratedTrees.Single
            (t => t.FilePath.EndsWith("PasteWindowViewModel.Deserializers.g.cs"));
        Assert.Equal(ExpectedGeneratedText, generated.GetText().ToString(),
            ignoreLineEndingDifferences: true);
    }
}
