using RetailCorrector.Controls;
using RetailCorrector.Utils;
using System.Windows;

namespace RetailCorrector.Actions.RemoveReceipt
{
    [RegisterAction("Удаление чеков")]
    internal partial class RemoveReceiptPresenter : ActionPresenter<RemoveReceiptPresenter, RemoveReceiptView>
    {
        private static RemoveReceiptPresenter instance;
        public ConditionGrid Condition = new();

        public RemoveReceiptPresenter()
        {
            instance = this;
        }

        public override void Start()
        {
            var count = Condition.DoByIndex(i => Workspace.Instance.Receipts.RemoveAt(i), _isBlacklist);
            MessageBox.Show($"{LocaleHelper.ConjugateWord(count, "Был удален", "Было удалено")} {count} {LocaleHelper.ConjugateWord(count, "чек", "чека", "чеков")}");
        }
    }
}
