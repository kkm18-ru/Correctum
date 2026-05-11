using Correctum.Fptr.Enums;

namespace Correctum.Fptr.Functions;

public sealed class Session : BaseFunctions
{
    internal Session(nint handle) : base(handle) { }

    public SessionState State
    {
        get
        {
            ObjectDisposedException.ThrowIf(disposed, this);
            FptrLibrary.Instance.SetParamInt(handle, 65587, 14);
            FptrException.ThrowIfFailedQueryData(handle);
            return (SessionState)FptrLibrary.Instance.GetParamInt(handle, 65592);
        }
    }

    public bool Open(bool silent)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        FptrLibrary.Instance.SetParamBool(handle, 65749, silent);
        return FptrLibrary.Instance.OpenShift(handle) == 0;
    }

    public bool Close(bool silent)
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        FptrLibrary.Instance.SetParamInt(handle, 65546, 0);
        FptrLibrary.Instance.SetParamBool(handle, 65749, silent);
        return FptrLibrary.Instance.Report(handle) == 0;
    }
}
