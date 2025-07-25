using RetailCorrector.Utils;

namespace RetailCorrector.Parsers
{
    internal partial class Parser
    {
        public RelayCommand ParseCommand = new(async _ =>
        {
            Instance!.CancellationSource = new();
            dynamic child = Instance.Child!;
            var parser = (IParserPresenter)child.Presenter;
            await foreach(var receipt in parser.Parse(Instance.CancellationSource.Token))
                Workspace.Instance.Receipts.Add(receipt);
            Instance.CancellationSource = null;
            ParserWindow.Instance!.Close();
        });

        public RelayCommand CancelCommand = new(_ =>
        {
            Instance!.CancellationSource!.Cancel();
            Instance.CancellationSource = null;
        });
    }
}
