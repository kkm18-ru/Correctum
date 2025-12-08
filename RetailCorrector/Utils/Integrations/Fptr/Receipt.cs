using RetailCorrector.Data;
using RetailCorrector.Utils.Extensions;
using System.Windows;

namespace RetailCorrector.Utils.Integrations
{
    internal sealed partial class FptrHandle
    {
        public bool OpenReceipt(Receipt doc, OpenReceiptArgs args)
        {
            var corrData = Array.Empty<byte>();
            if(docFormat >= 110) GenerateCorrectionData(doc.Created, args.Act, out corrData);
            var opcode = doc.Type.ToCorrectionOperation(docFormat);
            if (opcode is null) return false;
            SetInteger(65545, opcode.Value);
            SetBoolean(65572, !args.Print);
            SetInteger(1055, (uint)doc.TaxationSystem);

            if (args.IsInternetPayment)
            {
                SetBoolean(1125, true);
                SetString(1008, doc.BuyerContact);
                SetString(1187, args.HttpAddress);
            }
            if (docFormat >= 110) // todo add for ffd 1.05
            {
                SetInteger(1173, (uint)args.CorrType);
                SetBytes(1174, corrData);
            }
            SetString(1192, doc.DocSign.ToString());
            return openReceipt(handle) == 0;

        }

        public bool GenerateCorrectionData(DateTime dtOriginal, string actNumber, out byte[] corrData)
        {
            corrData = [];
            SetDate(1178, dtOriginal);
            SetString(1179, actNumber);
            if (!UtilFromTLV()) return false;
            corrData = GetBytes(65624);
            return true;
        }

        public bool AddPosition(Position pos)
        {
            SetString(65631, pos.Name);
            SetDouble(65632, pos.Price / 100.0);
            SetDouble(65633, pos.Quantity / 1000.0);
            SetDouble(65634, pos.Cost / 100.0);
            SetInteger(65569, pos.TaxRate.ToFptr());
            SetInteger(1212, (uint)pos.Type);
            SetInteger(1214, (uint)pos.PayMethod);
            if (docFormat == 120)
                SetInteger(2108, (uint)pos.MeasureUnit!);
            return registration(handle) == 0;
        }

        public bool SetTotal(double sum)
        {
            SetDouble(65613, sum);
            return receiptTotal(handle) == 0;
        }

        public bool Payment(PaymentSum data)
        {
            SetInteger(65564, (uint)data.Type);
            SetDouble(65565, data.Sum / 100.0);
            return payment(handle) == 0;
        }

        public bool Payment(PaymentSum[] array)
        {
            foreach(var data in array)
                if (!Payment(data)) 
                    return false;
            return true;
        }

        public bool CloseReceipt() => closeReceipt(handle) == 0;

        public void CancelReceipt()
        {
            var (code, desc) = (ErrorCode, ErrorDescription);
            cancelReceipt(handle);
            if (code == 0) return;
            var title = $"Произошла ошибка (Код: {code})";
            var text = $"{desc}\n\nЖелаете выполнить авто-исправление?";
            var resp = MessageBox.Show(text, title, MessageBoxButton.YesNo, MessageBoxImage.Hand);
            if (resp == MessageBoxResult.Yes && TryFixError()) return;
            Disconnect();
            text = $"{desc}\n\nДрайвер освободил кассу для её настройки, закройте данное окно для продолжения пробития!";
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Hand);
            Connect();
        }
    }

    public struct OpenReceiptArgs
    {
        public bool IsInternetPayment { get; set; }
        public string HttpAddress { get; set; }
        public string Act { get; set; }
        public bool Print { get; set; }
        public int CorrType { get; set; }
    }
}
