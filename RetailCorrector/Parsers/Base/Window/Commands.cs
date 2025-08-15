using RetailCorrector.Controls;
using RetailCorrector.Utils;
using System.Windows;

namespace RetailCorrector.Parsers
{
    internal partial class Parser
    {
        public RelayCommand ParseCommand = new(async _ =>
        {
            Instance!.CancellationSource = new();
            var presenter = (IParserPresenter)((dynamic)Instance.Child!).Presenter;
            try
            {
                await foreach (var receipt in presenter.Parse(Instance.CancellationSource.Token))
                    Workspace.Instance.Receipts.Add(receipt);
                Instance.CancellationSource = null;
                ParserWindow.Instance!.Close();
                ReceiptPanel.UpdateReceiptList();
            }
            catch(Exception e)
            {
                if (e is OperationCanceledException || e is TaskCanceledException)
                {
                    var lastParse = Workspace.Instance.ParserHistory[^1];
                    for (var i = Workspace.Instance.Receipts.Count - 1; i >= 0; i--)
                    {
                        if (Workspace.Instance.Receipts[i].ParseId == lastParse.Id)
                            Workspace.Instance.Receipts.RemoveAt(i);
                    }
                    Workspace.Instance.ParserHistory.Remove(lastParse);
                    MessageBox.Show("Выгрузка чеков была отменена пользователем");
                }
                else throw;
            }
        });

        public RelayCommand CancelCommand = new(_ =>
        {
            Instance!.CancellationSource!.Cancel();
            Instance.CancellationSource = null;
        });
    }
}
