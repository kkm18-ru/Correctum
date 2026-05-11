using Correctum.Fptr.Enums;

namespace Correctum.Fptr.Data;

public readonly struct CorrectionBasis
{
    public FiscalDocumentFormat FiscalFormat { get; }
    public CorrectionReason Reason { get; }
    public DateTime DocDateTime { get; }
    public string DocNumber { get; }
    public CorrectionType Type { get; }

    public byte[] ToBytes(nint handle)
    {
        if (FiscalFormat <= FiscalDocumentFormat.FFD_1_05 &&
            Reason == CorrectionReason.FixingErrors) return [];
        FptrLibrary.Instance.SetParamDateTime(handle, 1178, DocDateTime);
        FptrLibrary.Instance.SetParamString(handle, 1179, DocNumber);
        FptrException.ThrowIfFailedUtilFormTLV(handle);
        return FptrLibrary.Instance.GetParamBytes(handle, 65624);
    }
}
