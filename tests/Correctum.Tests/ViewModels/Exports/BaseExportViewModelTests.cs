using Correctum.ViewModels.Exports;
using System.Windows;

namespace Correctum.Tests.ViewModels.Exports;

public class BaseExportViewModelTests
{
    [Test] public async Task ThrowException()
    {
        var viewModel = new MockExportViewModel { Message = "Test Error" };
        viewModel.NotifyCreated += (message, caption, _, img) =>
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(message, Is.EqualTo("Test Error"));
                Assert.That(caption, Is.EqualTo("Не удалось экспортировать ФД."));
                Assert.That(img, Is.EqualTo(MessageBoxImage.Error));
            }
        }; 
        viewModel.Export();
    }

    [Test] public async Task Success()
    {
        var viewModel = new MockExportViewModel();
        viewModel.NotifyCreated += 
            (m, _, _, _) => Assert.That(m, Is.EqualTo("Экспорт завершен успешно"));
        viewModel.Export();
    }

    [Test] public async Task Force()
    {
        var viewModel = new MockExportViewModel();
        viewModel.NotifyCreated += (_, _, _, _) => Assert.Fail();
        viewModel.Export();
    }
}

file class MockExportViewModel : BaseExportViewModel
{
    public override string ExportName { get; } = "Tests";
    public string? Message { private get; set; }

    protected override async Task<bool> InternalExportAsync()
    {
        if (Message is not null) throw new Exception(Message);
        return true;
    }
}