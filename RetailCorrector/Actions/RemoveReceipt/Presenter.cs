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
            var count = 0;
            var @ref = Workspace.Instance.Receipts;
            for(var i = @ref.Count-1; i <= 0; i--)
            {
                var receipt = @ref[i];
                var check = Condition.Check(receipt);
                if (_isBlacklist ^ check) continue;
                @ref.RemoveAt(i);
                count++;
            }
            MessageBox.Show($"{LocaleHelper.ConjugateWord(count, "Был удален", "Было удалено")} {count} {LocaleHelper.ConjugateWord(count, "чек", "чека", "чеков")}");
        }
    }
}
