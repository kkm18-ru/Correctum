using RetailCorrector.UI.Controls;
using RetailCorrector.Utils;

namespace RetailCorrector.Actions
{
    internal partial class RemoveReceipt : ActionBase<RemoveReceipt, RemoveReceipt.View>
    {
        public ConditionGrid Condition = new();

        public override string Name => "Удаление чеков";

        public override void Start()
        {
            var count = Condition.DoByIndex(Workspace.Instance.Receipts.RemoveAt);
            ReceiptPanel.UpdateReceiptList();
            System.Windows.MessageBox.Show($"{LocaleHelper.ConjugateWord(count, "Был удален", "Было удалено")} {count} {LocaleHelper.ConjugateWord(count, "чек", "чека", "чеков")}");
        }
    }
}
