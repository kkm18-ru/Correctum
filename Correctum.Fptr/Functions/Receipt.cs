using Correctum.Core.Models;
using Correctum.Core.ValueObjects;
using Correctum.Fptr.Data;
using Correctum.Fptr.Enums;
using Correctum.Fptr.Extensions;

namespace Correctum.Fptr.Functions;

public sealed class Receipt : BaseFunctions
{
    internal Receipt(nint handle) : base(handle) { }

    public uint LastReceiptNumber
    {
        get
        {
            FptrLibrary.Instance.SetParamInt(handle, 65622, 4);
            FptrException.ThrowIfFailedFnQueryData(handle);
            return FptrLibrary.Instance.GetParamInt(handle, 65598);
        }
    }

    // todo Get Receipt 

    public void Open(Core.Models.Receipt receipt, CorrectionBasis corrBasis)
    {
        var corrBytes = corrBasis.ToBytes(handle);
        var operation = CoreHelpers.ConvertOperation(receipt.Type!.Value, corrBasis);
        FptrLibrary.Instance.SetParamInt(handle, 65542, operation);
        FptrLibrary.Instance.SetParamBool(handle, 65572, true);
        if (!receipt.TaxationSystem.HasValue)
            FptrLibrary.Instance.SetParamInt(handle, 1055, (uint)receipt.TaxationSystem!.Value);
        // todo 1125 - IsInternetPayment
        // todo 1187 - Address
        if (!string.IsNullOrWhiteSpace(receipt.BuyerContact))
            FptrLibrary.Instance.SetParamString(handle, 1008, receipt.BuyerContact);
        else
        {
            FptrLibrary.Instance.SetParamString(handle, 1008, "none");
            FptrLibrary.Instance.SetParamString(handle, 1117, "none");
        }

        if (corrBytes.Length > 0) { 
            FptrLibrary.Instance.SetParamInt(handle, 1173, (uint)corrBasis.Type);
            FptrLibrary.Instance.SetParamByteArray(handle, 1174, corrBytes, corrBytes.Length);
        }

        if (corrBasis.Reason == CorrectionReason.FixingErrors)
            FptrLibrary.Instance.SetParamString(handle, 1192, receipt.DocSign!.Value.ToString());
        FptrException.ThrowIfFailed(FptrLibrary.Instance.OpenReceipt, handle);
    }

    public void RegistrationPosition(Position position, FiscalDocumentFormat ffd)
    {
        FptrLibrary.Instance.SetParamString(handle, 65631, position.Name!);
        FptrLibrary.Instance.SetParamDouble(handle, 65632, (double)position.Price!.Value);
        FptrLibrary.Instance.SetParamDouble(handle, 65633, (double)position.Quantity!.Value);
        FptrLibrary.Instance.SetParamDouble(handle, 65634, (double)position.Cost!.Value);
        FptrLibrary.Instance.SetParamInt(handle, 65569, (uint)position.TaxRate!);
        FptrLibrary.Instance.SetParamInt(handle, 1212, (uint)position.Type!);
        FptrLibrary.Instance.SetParamInt(handle, 1214, (uint)position.CalcMethod!);
        if (ffd == FiscalDocumentFormat.FFD_1_2)
            FptrLibrary.Instance.SetParamInt(handle, 2108, (uint)position.Unit!);
        FptrException.ThrowIfFailed(FptrLibrary.Instance.Registration, handle);
    }

    public void SetTotalSum(Currency sum)
    {
        FptrLibrary.Instance.SetParamDouble(handle, 65613, (double)sum);
        FptrException.ThrowIfFailed(FptrLibrary.Instance.ReceiptTotal, handle);
    }

    public void Payment(PaymentType type, Currency? sum)
    {
        if (sum is null || (ulong)sum.Value == 0) return;
        FptrLibrary.Instance.SetParamInt(handle, 65564, (uint)type);
        FptrLibrary.Instance.SetParamDouble(handle, 65565, (double)sum);
        FptrException.ThrowIfFailed(FptrLibrary.Instance.Payment, handle);
    }

    public void Payment(Payment data)
    {
        Payment(PaymentType.Cash, data.CashSum);
        Payment(PaymentType.NonCash, data.NonCashSum);
        Payment(PaymentType.Advance, data.AdvanceSum);
        Payment(PaymentType.Credit, data.CreditSum);
        Payment(PaymentType.Provision, data.ProvisionSum);
    }

    public void Close() =>
        FptrException.ThrowIfFailed(FptrLibrary.Instance.CloseReceipt, handle);
}
