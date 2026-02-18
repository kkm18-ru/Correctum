using RetailCorrector.UI.Controls;
using System.Windows;

namespace RetailCorrector.Actions
{
    internal partial class CancelReceipt : ActionBase<CancelReceipt>
    {
        public override string Name => "Генерация аннулирующих чеков";
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public override void Start()
        {
            if(Workspace.Instance.Receipts.Any(r => r.EditsCount != 0))
            {
                Logger.Error("Генерация аннулирующих чеков, используя не перноначальные данные, запрещена!");
                MessageBox.Show("Генерация аннулирующих чеков, используя не перноначальные данные, запрещена!");
                return;
            }
            var count = Workspace.Instance.Receipts.Count;
            for(var i = 0; i < count; i++)
            {
                var receipt = Workspace.Instance.Receipts[i*2];
                Workspace.Instance.Receipts.Insert(i * 2 + 1, receipt with 
                { 
                    Type = receipt.Type switch
                    {
                        Enums.Operation.RefundOutcome => Enums.Operation.Outcome,
                        Enums.Operation.Outcome => Enums.Operation.RefundOutcome,
                        Enums.Operation.Income => Enums.Operation.RefundIncome,
                        Enums.Operation.RefundIncome => Enums.Operation.Income,
                    }
                });
            }
            Logger.Info($"{string.Conjugate(count, "Был сгенерирован", "Было сгенерировано")} {{Count}} {string.Conjugate(count, "чек", "чека", "чеков")}", count);
            ReceiptPanel.UpdateReceiptList();            
            MessageBox.Show($"{string.Conjugate(count, "Был сгенерирован", "Было сгенерировано")} {count} {string.Conjugate(count, "чек", "чека", "чеков")}");
        }
    }
}
