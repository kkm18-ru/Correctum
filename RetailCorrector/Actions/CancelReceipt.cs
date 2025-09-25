using RetailCorrector.Attributes;
using RetailCorrector.Data;
using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;
using Serilog;
using System.Windows;

namespace RetailCorrector.Actions
{
    [RegisterAction(DisplayName = "Генерация аннулирующих чеков")]
    internal class CancelReceipt : ActionBase<CancelReceipt>
    {
        public override void Start()
        {
            if(Workspace.Instance.Receipts.Any(r => r.EditsCount != 0))
            {
                Log.Error("Генерация аннулирующих чеков, используя не перноначальные данные, запрещена!");
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
                    Type = receipt.Type switch
                    {
                        Enums.Operation.RefundOutcome => Enums.Operation.Outcome,
                        Enums.Operation.Outcome => Enums.Operation.RefundOutcome,
                        Enums.Operation.Income => Enums.Operation.RefundIncome,
                        Enums.Operation.RefundIncome => Enums.Operation.Income,
                    },
                    TotalSum = receipt.TotalSum
                });
            }
            Log.Information($"{LocaleHelper.ConjugateWord(count, "Был сгенерирован", "Было сгенерировано")} {{Count}} {LocaleHelper.ConjugateWord(count, "чек", "чека", "чеков")}", count);
            ReceiptPanel.UpdateReceiptList();            
            MessageBox.Show($"{LocaleHelper.ConjugateWord(count, "Был сгенерирован", "Было сгенерировано")} {count} {LocaleHelper.ConjugateWord(count, "чек", "чека", "чеков")}");
        }
    }
}
