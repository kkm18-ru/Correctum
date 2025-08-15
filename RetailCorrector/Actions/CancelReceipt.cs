using RetailCorrector.Controls;
using RetailCorrector.Data;
using RetailCorrector.Utils;
using System.Windows;

namespace RetailCorrector.Actions.CancelReceipt
{
    [RegisterAction("Генерация аннулирующих чеков")]
    internal class CancelReceipt : ActionPresenter<CancelReceipt>
    {
        public override void Start()
        {
            if(Workspace.Instance.Receipts.Any(r => r.EditsCount != 0))
            {
                MessageBox.Show("Генерация аннулирующих чеков, используя не перноначальные данные, запрещена!");
                return;
            }
            var count = Workspace.Instance.Receipts.Count;
            for(var i = 0; i < count; i++)
            {
                var receipt = Workspace.Instance.Receipts[i*2];
                Workspace.Instance.Receipts.Insert(i * 2 + 1, new Receipt(true)
                {
                    ParseId = receipt.ParseId,
                    Created = receipt.Created,
                    DocSign = receipt.DocSign,
                    Items = (Position[])receipt.Items.Clone(),
                    Payment = receipt.Payment,
                    Type = receipt.Type switch // todo в 1.05 in -> out -> in
                    {
                        Enums.Operation.RefundOutcome => Enums.Operation.Outcome,
                        Enums.Operation.Outcome => Enums.Operation.RefundOutcome,
                        Enums.Operation.Income => Enums.Operation.RefundIncome,
                        Enums.Operation.RefundIncome => Enums.Operation.Income,
                    },
                    TotalSum = receipt.TotalSum
                });
            }
            ReceiptPanel.UpdateReceiptList();
            MessageBox.Show($"{LocaleHelper.ConjugateWord(count, "Был сгенерирован", "Было сгенерировано")} {count} {LocaleHelper.ConjugateWord(count, "чек", "чека", "чеков")}");
        }
    }
}
