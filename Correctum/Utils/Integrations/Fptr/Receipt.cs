using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using System.Windows;

namespace Correctum.Utils.Integrations
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

        public bool IsOpenedDocument
        {
            get
            {
                while (!CheckDocumentClosed())
                    if (CheckDocumentClosed())
                        break;
                return GetBoolean(65644);
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
            if (!LoginOperator(doc.OperatorName!, doc.OperatorInn?.ToString() ?? "")) return false;
            if (DocFormat >= 110) GenerateCorrectionData(doc.Created!.Value, args.Act, out corrData);
            var opcode = doc.Type!.Value.ToCorrection(DocFormat);
            if (opcode is null) return false;
            SetInteger(65545, opcode.Value);
            SetBoolean(65572, true);
            if(doc.TaxationSystem is not null)
                SetInteger(1055, (uint)doc.TaxationSystem);
            if(doc.RetailPlace is not null) 
                SetString(1187, doc.RetailPlace);
            if(doc.RetailAddress is not null) 
                SetString(1009, doc.RetailAddress);

            if (args.IsInternetPayment)
            {
                SetBoolean(1125, true);
                SetString(1008, doc.BuyerContact!);
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
            if(doc.DocSign is { } sign) 
                SetString(1192, sign.ToString());
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
            SetString(65631, pos.Name!);
            SetDouble(65632, (double)pos.Price!.Value);
            SetDouble(65633, (double)pos.Quantity!);
            SetDouble(65634, (double)pos.Cost!.Value);
            SetInteger(65569, (uint)pos.TaxRate!);
            SetInteger(1212, (uint)pos.Type!);
            SetInteger(1214, (uint)pos.CalcMethod!);
            if (DocFormat == 120)
                SetInteger(2108, (uint)pos.Unit!);
            return registration(handle) == 0;
        }

        public bool SetTotal(Currency sum)
        {
            SetDouble(65613, (double)sum);
            return receiptTotal(handle) == 0;
        }

        public bool Payment(uint type, Currency? sum)
        {
            if (sum is null || (ulong)sum.Value == 0) return true;
            SetInteger(65564, type);
            SetDouble(65565, (double)sum.Value);
            return payment(handle) == 0;
        }

        public bool Payment(Receipt receipt) =>
            Payment(0, receipt.CashSum) &&
            Payment(1, receipt.NonCashSum) &&
            Payment(2, receipt.AdvanceSum) &&
            Payment(3, receipt.CreditSum) &&
            Payment(4, receipt.ProvisionSum);

        public bool CloseReceipt() => closeReceipt(handle) == 0;

        public void CancelReceipt()
        {
            if (!IsOpenedDocument) return;
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

    public readonly struct OpenReceiptArgs(bool internet, string act, int corr)
    {
        public bool IsInternetPayment { get; } = internet;
        public string Act { get; } = string.IsNullOrWhiteSpace(act) ? " " : act;
        public int CorrType { get; } = corr;
    }
}
