using RetailCorrector.Controls;

namespace RetailCorrector.Actions
{
    internal class SummaryReceiptView : ActionView<SummaryReceiptView, SummaryReceiptPresenter>;

    [RegisterAction("Суммирование чеков")]
    internal class SummaryReceiptPresenter : ActionPresenter<SummaryReceiptPresenter, SummaryReceiptView>
    {
        public override void Start()
        {
            ulong inCash = 0;
            ulong outCash = 0;
            ulong inEcash = 0;
            ulong outEcash = 0;
            ulong inPre = 0;
            ulong outPre = 0;
            ulong inPost = 0;
            ulong outPost = 0;
            ulong @in = 0;
            ulong @out = 0;

            foreach(var r in Workspace.Instance.Receipts)
            {
                if(r.Type == Enums.Operation.Income || r.Type == Enums.Operation.RefundOutcome)
                {
                    inCash += r.Payment.Cash;
                    inEcash += r.Payment.ECash;
                    inPre += r.Payment.Pre;
                    inPost += r.Payment.Post;
                    @in += r.Payment.Provision;
                }
                else
                {
                    outCash += r.Payment.Cash;
                    outEcash += r.Payment.ECash;
                    outPre += r.Payment.Pre;
                    outPost += r.Payment.Post;
                    @out += r.Payment.Provision;
                }
            }

            Workspace.Instance.Receipts.Clear();
            Workspace.Instance.Receipts.Add(new()
            {
                ParseId = Guid.Empty,
                Created = DateTime.Now,
                Type = Enums.Operation.Income,
                DocSign = 0,
                TotalSum = @in + inCash + inEcash + inPre+ inPost,
                Payment = new()
                {
                    Cash = inCash,
                    ECash = inEcash,
                    Post = inPost,
                    Pre = inPre,
                    Provision = @in
                },
                Items = [new() {
                    Name = "Товар",
                    Cost = @in + inCash + inEcash + inPre+ inPost,
                    Quantity = 1000,
                    Price = @in + inCash + inEcash + inPre+ inPost,
                    MeasureUnit = null,
                    Type = 0,
                    PayMethod = Enums.PayMethod.FullPayment,
                    TaxRate = Enums.TaxRate.Without
                }]
            });
            Workspace.Instance.Receipts.Add(new()
            {
                ParseId = Guid.Empty,
                Created = DateTime.Now,
                Type = Enums.Operation.Outcome,
                DocSign = 0,
                TotalSum = @out + outCash + outEcash + outPre + outPost,
                Payment = new()
                {
                    Cash = outCash,
                    ECash = outEcash,
                    Post = outPost,
                    Pre = outPre,
                    Provision = @out
                },
                Items = [new() {
                    Name = "Товар",
                    Cost = @out + outCash + outEcash + outPre + outPost,
                    Quantity = 1000,
                    Price = @out + outCash + outEcash + outPre + outPost,
                    MeasureUnit = null,
                    Type = 0,
                    PayMethod = Enums.PayMethod.FullPayment,
                    TaxRate = Enums.TaxRate.Without
                }]
            });

            ReceiptPanel.UpdateReceiptList();
        }
    }
}
