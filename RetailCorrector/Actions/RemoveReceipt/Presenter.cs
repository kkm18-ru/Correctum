using RetailCorrector.Controls;
using RetailCorrector.Utils;
using System.Windows;

namespace RetailCorrector.Actions.RemoveReceipt
{
    [RegisterAction("Удаление чеков")]
    internal partial class RemoveReceiptPresenter : ActionPresenter<RemoveReceiptPresenter, RemoveReceiptView>
    {
        public ConditionGrid Condition = new();

        public override void Start()
        {
            var count = Condition.DoByIndex(Workspace.Instance.Receipts.RemoveAt);
            MessageBox.Show($"{LocaleHelper.ConjugateWord(count, "Был удален", "Было удалено")} {count} {LocaleHelper.ConjugateWord(count, "чек", "чека", "чеков")}");
        }
    }
}
