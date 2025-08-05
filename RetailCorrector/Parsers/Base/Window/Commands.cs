using RetailCorrector.Utils;

namespace RetailCorrector.Parsers
{
    internal partial class Parser
    {
        public RelayCommand ParseCommand = new(async _ =>
        {
            Instance!.CancellationSource = new();
            var presenter = (ParserPresenter)((dynamic)Instance.Child!).Presenter;
            await foreach(var receipt in presenter.Parse(Instance.CancellationSource.Token))
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
