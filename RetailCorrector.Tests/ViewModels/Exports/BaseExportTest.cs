using RetailCorrector.ViewModels.Exports;
using System.Runtime.CompilerServices;

namespace RetailCorrector.Tests.ViewModels.Exports;

public class BaseExportTest<TViewModel, TTests>
    where TViewModel : BaseExportViewModel, new()
    where TTests : BaseExportTest<TViewModel, TTests>
{
    protected static TViewModel ViewModel { get; private set; }
    protected static string DirectoryPath { get; private set; }

    [OneTimeSetUp]
    public static void SetUp()
    {
        ViewModel = Activator.CreateInstance<TViewModel>();
        DirectoryPath = Path.Combine(Path.GetTempPath(), typeof(TTests).Assembly.GetName().Name, typeof(TTests).Name);
        if (!Directory.Exists(Path.GetDirectoryName(DirectoryPath))) Directory.CreateDirectory(Path.GetDirectoryName(DirectoryPath));
        if (!Directory.Exists(DirectoryPath)) Directory.CreateDirectory(DirectoryPath);
    }

    protected static string GenPath([CallerMemberName] string? methodName = null) =>
        Path.Combine(DirectoryPath, $"{methodName ?? Path.GetRandomFileName()}.js");

    [OneTimeTearDown]
    public static void TearDown()
    {
        Directory.Delete(DirectoryPath, true);
    }
}
