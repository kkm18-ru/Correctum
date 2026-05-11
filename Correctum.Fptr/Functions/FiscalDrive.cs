using Correctum.Fptr.Enums;

namespace Correctum.Fptr.Functions;

public sealed class FiscalDrive : BaseFunctions
{
    internal FiscalDrive(nint handle) : base(handle) { }

    public string SerialNumber
    {
        get
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            FptrLibrary.Instance.SetParamInt(handle, 65622, 2);
            FptrException.ThrowIfFailedFnQueryData(handle);
            return FptrLibrary.Instance.GetParamString(handle, 65559);
        }
    }

    public FiscalDocumentFormat FiscalFormat
    {
        get
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            FptrLibrary.Instance.SetParamInt(handle, 65622, 7);
            FptrException.ThrowIfFailedFnQueryData(handle);
            return (FiscalDocumentFormat)FptrLibrary.Instance.GetParamInt(handle, 65628);
        }
    }

    public bool OperatorLogin(string operatorFullName, string inn = "")
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        FptrLibrary.Instance.SetParamString(handle, 1021, operatorFullName);
        if (!string.IsNullOrWhiteSpace(inn))
            FptrLibrary.Instance.SetParamString(handle, 1203, inn);
        return FptrLibrary.Instance.OperatorLogin(handle) == 0;
    }
}
