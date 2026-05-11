using Correctum.Core.Enums;
using Correctum.Fptr.Data;
using Correctum.Fptr.Enums;

namespace Correctum.Fptr.Extensions
{
    public static class CoreHelpers
    {
        public static uint ConvertOperation(ReceiptType operation, CorrectionBasis corrBasis) => 
            (corrBasis.FiscalFormat, corrBasis.Reason, operation) switch
            {
                (FiscalDocumentFormat.FFD_1_1 or FiscalDocumentFormat.FFD_1_2, _, ReceiptType.Income) => 7,
                (FiscalDocumentFormat.FFD_1_1 or FiscalDocumentFormat.FFD_1_2, _, ReceiptType.RefundIncome) => 8,
                (FiscalDocumentFormat.FFD_1_1 or FiscalDocumentFormat.FFD_1_2, _, ReceiptType.Outcome) => 9,
                (FiscalDocumentFormat.FFD_1_1 or FiscalDocumentFormat.FFD_1_2, _, ReceiptType.RefundOutcome) => 10,

                (FiscalDocumentFormat.FFD_1_05, CorrectionReason.FixingErrors, ReceiptType.Income) => 1,
                (FiscalDocumentFormat.FFD_1_05, CorrectionReason.FixingErrors, ReceiptType.RefundIncome) => 2,
                (FiscalDocumentFormat.FFD_1_05, CorrectionReason.FixingErrors, ReceiptType.Outcome) => 4,
                (FiscalDocumentFormat.FFD_1_05, CorrectionReason.FixingErrors, ReceiptType.RefundOutcome) => 5,

                (FiscalDocumentFormat.FFD_1_05, _, ReceiptType.Outcome or ReceiptType.RefundIncome) => 7,
                (FiscalDocumentFormat.FFD_1_05, _, ReceiptType.RefundOutcome or ReceiptType.Income) => 9,

                (FiscalDocumentFormat.FFD_1_0, _, _) => throw new NotSupportedException("В ФФД 1.0 отсутствует поддержка коррекции чеков."),
                (FiscalDocumentFormat.Unknown, _, _) => throw new InvalidOperationException("Неизвестная версия ФФД."),
                (FiscalDocumentFormat ffd, _, _) => throw new InvalidOperationException($"Неожиданная версия ФФД ({ffd})."),
            };
    }
}
