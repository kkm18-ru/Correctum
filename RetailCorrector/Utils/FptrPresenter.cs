using System.Windows;
using Atol.Drivers10.Fptr;
using RetailCorrector.Data;
using RetailCorrector.Enums;

namespace RetailCorrector.Utils
{
    internal static class FptrPresenter
    {
        public static void WaitClearBuffer(IFptr fptr, int max, int record)
        {
            var count = GetUnsentDocuments(fptr);
            if (count < max) return;
            while(count > record)
            {
                Thread.Sleep(1000);
                count = GetUnsentDocuments(fptr);
            }
        }

        private static uint GetUnsentDocuments(IFptr fptr)
        {
            fptr.setParam(Constants.LIBFPTR_PARAM_FN_DATA_TYPE, Constants.LIBFPTR_FNDT_ISM_EXCHANGE_STATUS);
            fptr.fnQueryData();
            return fptr.getParamInt(Constants.LIBFPTR_PARAM_DOCUMENTS_COUNT);
        }

        public static uint? GetSessionState(IFptr fptr)
        {
            fptr.setParam(Constants.LIBFPTR_PARAM_DATA_TYPE, Constants.LIBFPTR_DT_SHIFT_STATE);
            if(fptr.queryData() != 0) return null;
            return fptr.getParamInt(Constants.LIBFPTR_PARAM_SHIFT_STATE);
        }

        public static uint? GetFFD(IFptr fptr)
        {
            fptr.setParam(Constants.LIBFPTR_PARAM_FN_DATA_TYPE, Constants.LIBFPTR_FNDT_FFD_VERSIONS);
            if(fptr.fnQueryData() != 0) return null;
            var fn = fptr.getParamInt(Constants.LIBFPTR_PARAM_FN_FFD_VERSION);
            var dev = fptr.getParamInt(Constants.LIBFPTR_PARAM_FFD_VERSION);
            return (fn, dev) switch
            {
                (0, _) => dev, (_, 0) => fn,
                (_, _) => Math.Min(fn, dev),
            };
        }

        public static bool AddPosition(IFptr fptr, Position pos, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var ffd = GetFFD(fptr);
            fptr.setParam(Constants.LIBFPTR_PARAM_COMMODITY_NAME, pos.Name);
            fptr.setParam(Constants.LIBFPTR_PARAM_PRICE, pos.Price / 100.0);
            fptr.setParam(Constants.LIBFPTR_PARAM_QUANTITY, pos.Quantity / 1000.0);
            fptr.setParam(Constants.LIBFPTR_PARAM_POSITION_SUM, pos.Cost / 100.0);
            fptr.setParam(Constants.LIBFPTR_PARAM_TAX_TYPE, (uint)pos.TaxRate);
            fptr.setParam(1212, (uint)pos.Type);
            fptr.setParam(1214, (uint)pos.PayMethod);
            if(ffd == 120) fptr.setParam(2108, (uint)pos.MeasureUnit!);
            return 0 == fptr.registration();
        }

        public static bool Payment(IFptr fptr, Payment pay) =>
            SinglePayment(fptr, 0, pay.Cash) && SinglePayment(fptr, 1, pay.ECash) &&
            SinglePayment(fptr, 2, pay.Pre) && SinglePayment(fptr, 3, pay.Post) &&
            SinglePayment(fptr, 4, pay.Provision);

        private static bool SinglePayment(IFptr fptr, uint type, ulong sum)
        {
            if (sum == 0) return true;
            fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_TYPE, type);
            fptr.setParam(Constants.LIBFPTR_PARAM_PAYMENT_SUM, sum/100.0);
            return 0 == fptr.payment();
        }

        public static bool TotalReceipt(IFptr fptr, ulong total)
        {
            fptr.setParam(Constants.LIBFPTR_PARAM_SUM, total / 100.0);
            return 0 == fptr.receiptTotal();
        }

        public struct OpenReceiptArgs
        {
            public string Act { get; set; }
            public bool Print { get; set; }
            public int CorrType { get; set; }
        }
        
        public static bool OpenReceipt(IFptr fptr, Receipt doc, OpenReceiptArgs args)
        {
            var ffd = GetFFD(fptr);
            byte[] corrData = [];

            if(ffd >= 110)  // todo add for ffd 1.05
            {
                fptr.setParam(1178, doc.Created);
                fptr.setParam(1179, args.Act);
                fptr.utilFormTlv();
                corrData = fptr.getParamByteArray(Constants.LIBFPTR_PARAM_TAG_VALUE);
            }

            var opcode = GetOperation(fptr, doc.Type);
            if (opcode is null) return false;
            fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_TYPE, (uint)opcode);
            fptr.setParam(Constants.LIBFPTR_PARAM_RECEIPT_ELECTRONICALLY, args.Print);

            if (ffd >= 110) // todo add for ffd 1.05
            {
                fptr.setParam(1173, args.CorrType);
                fptr.setParam(1174, corrData);
            }
            fptr.setParam(1192, doc.DocSign.ToString());
            return 0 == fptr.openReceipt();
        }

        public static void CancelReceipt(IFptr fptr)
        {
            var code = fptr.errorCode();
            var desc = fptr.errorDescription();
            fptr.cancelReceipt();
            fptr.close();
            var text = $"{desc}\n\nДрайвер освободил кассу для её настройки, закройте данное окно для продолжения пробития!";
            var title = $"Произошла ошибка (Код: {code})";
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Hand);
            fptr.open();
        }

        private static int? GetOperation(IFptr fptr, Operation opcode) => (GetFFD(fptr), opcode) switch
        {
            (null, _) => null,
            (<= 105, Operation.Income or Operation.RefundOutcome) => Constants.LIBFPTR_RT_SELL,
            (<= 105, Operation.Outcome or Operation.RefundIncome) => Constants.LIBFPTR_RT_BUY,
            (>= 110, Operation.Income) => Constants.LIBFPTR_RT_SELL_CORRECTION,
            (>= 110, Operation.Outcome) => Constants.LIBFPTR_RT_BUY_CORRECTION,
            (>= 110, Operation.RefundOutcome) => Constants.LIBFPTR_RT_BUY_RETURN_CORRECTION,
            (>= 110, Operation.RefundIncome) => Constants.LIBFPTR_RT_SELL_RETURN_CORRECTION,
            _ => throw new NotImplementedException()
        };
    }
}
