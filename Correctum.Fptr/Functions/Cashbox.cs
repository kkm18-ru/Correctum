using Correctum.Fptr.Data;
using Correctum.Fptr.Enums;

namespace Correctum.Fptr.Functions;

public sealed class Cashbox : BaseFunctions
{
    internal Cashbox(nint handle) : base(handle) { }

    public string SerialNumber
    {
        get
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            FptrLibrary.Instance.SetParamInt(handle, 65587, 16); // DataType: SerialNumber
            FptrException.ThrowIfFailedQueryData(handle);
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
            return (FiscalDocumentFormat)FptrLibrary.Instance.GetParamInt(handle, 65629);
        }
    }

    public bool Connected
    {
        get
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            return FptrLibrary.Instance.IsOpened(handle);
        }
    }

    public bool Connect()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        return FptrLibrary.Instance.Open(handle) == 0;
    }

    public bool Disconnect()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        return FptrLibrary.Instance.Close(handle) == 0;
    }

    public License GetLicense(uint number)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        FptrLibrary.Instance.SetParamInt(handle, 65587, 4);
        FptrLibrary.Instance.SetParamInt(handle, 65610, number);
        FptrException.ThrowIfFailedQueryData(handle);
        return new License(handle);
    }

    public Version GetVersionUnit(UnitType unitType)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        FptrLibrary.Instance.SetParamInt(handle, 65587, 2);
        FptrLibrary.Instance.SetParamInt(handle, 65609, (uint)unitType);
        FptrException.ThrowIfFailedQueryData(handle);
        return Version.Parse(FptrLibrary.Instance.GetParamString(handle, 65604));
    }
}
