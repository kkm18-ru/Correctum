using RetailCorrector.Data;
using System.Windows;

namespace RetailCorrector.Utils.Integrations
{
    internal sealed partial class FptrHandle
    {
        public uint LastReceiptNumber
        {
            get
            {
                SetInteger(65622, 4);
                QueryFN();
                return GetInteger(65598);
            }
        }

        public (string, DateTime) GetReceiptInfo(uint number)
        {
            SetInteger(65622, 13);
            SetInteger(65598, number);
            QueryFN();
            return (GetString(65626), GetDate(65590));
        }
        
        public bool OpenReceipt(Receipt doc, OpenReceiptArgs args)
        {
            var corrData = Array.Empty<byte>();
            if(DocFormat >= 110) GenerateCorrectionData(doc.Created, args.Act, out corrData);
            var opcode = doc.Type.ToCorrection(DocFormat);
            if (opcode is null) return false;
            SetInteger(65545, opcode.Value);
            SetBoolean(65572, true);
            SetInteger(1055, (uint)doc.TaxationSystem);

            if (args.IsInternetPayment)
            {
                SetBoolean(1125, true);
                SetString(1008, doc.BuyerContact);
                SetString(1187, args.HttpAddress);
            }
            else if (ConfigurationVersion >= new Version(5, 17))
            {
                SetString(1008, "none");
                SetString(1117, "none");
            }
            
            if (DocFormat >= 110) // todo add for ffd 1.05
            {
                SetInteger(1173, (uint)args.CorrType);
                SetBytes(1174, corrData);
            }
            if(doc.DocSign > 0) 
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
            if (DocFormat == 120)
                SetInteger(2108, (uint)pos.MeasureUnit!);
            return registration(handle) == 0;
        }

        public bool SetTotal(double sum)
        {
            SetDouble(65613, sum);
            return receiptTotal(handle) == 0;
        }

        public bool Payment(uint type, double sum)
        {
            SetInteger(65564, type);
            SetDouble(65565, sum);
            return payment(handle) == 0;
        }

        public bool Payment(Payment pay) =>
            (pay.Cash == 0 || Payment(0, pay.Cash / 100.0)) &&
            (pay.ECash == 0 || Payment(1, pay.ECash / 100.0)) &&
            (pay.Pre == 0 || Payment(2, pay.Pre / 100.0)) &&
            (pay.Post == 0 || Payment(3, pay.Post / 100.0)) &&
            (pay.Provision == 0 || Payment(4, pay.Provision / 100.0));

        public bool CloseReceipt() => closeReceipt(handle) == 0;

        public void CancelReceipt()
        {
            var (code, desc) = (ErrorCode, ErrorDescription);
            cancelReceipt(handle);
            if (code == 0) return;
            if (AutoFixWoutAsk.Contains(code) && TryFixErrorWoutAsk(code)) return;
            var title = $"Произошла ошибка (Код: {code})";
            string text;
            if (AutoFixWithAsk.Contains(code))
            {
                text = $"{desc}\n\nЖелаете выполнить автоматическое исправление?";
                var resp = MessageBox.Show(text, title, MessageBoxButton.YesNo, MessageBoxImage.Hand);
                if (resp == MessageBoxResult.Yes && TryFixErrorWithAsk(code)) return;
            }
            Disconnect();
            text = $"{desc}\n\nДрайвер освободил кассу для её настройки, закройте данное окно для продолжения пробития!";
            MessageBox.Show(text, title, MessageBoxButton.OK, MessageBoxImage.Hand);
            Connect();
        }
    }

    public readonly struct OpenReceiptArgs(bool internet, string site, string act, int corr)
    {
        public bool IsInternetPayment { get; } = internet;
        public string HttpAddress { get; } = site;
        public string Act { get; } = string.IsNullOrWhiteSpace(act) ? " " : act;
        public int CorrType { get; } = corr;
    }
}
