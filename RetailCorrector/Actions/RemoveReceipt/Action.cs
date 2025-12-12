using RetailCorrector.UI.Controls;

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
            System.Windows.MessageBox.Show($"{string.Conjugate(count, "Был удален", "Было удалено")} {count} {string.Conjugate(count, "чек", "чека", "чеков")}");
        }
    }
}
