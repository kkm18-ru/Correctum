namespace Correctum.Fptr;

public sealed class FptrException : Exception
{
    public int ErrorCode { get; }

    internal FptrException(nint handle) : 
        base(GetErrorDescription(handle))
    {
        ErrorCode = FptrLibrary.Instance.ErrorCode(handle);
        FptrLibrary.Instance.CancelReceipt(handle);
    }

    private static string GetErrorDescription(nint handle)
    {
        var bytes = new byte[FptrLibrary.DefaultSizeBytes];
        var len = FptrLibrary.Instance.ErrorDescription(handle, bytes, bytes.Length);
        if (len > bytes.Length)
        {
            Array.Resize(ref bytes, len);
            FptrLibrary.Instance.ErrorDescription(handle, bytes, bytes.Length);
        }
        return FptrLibrary.Encoding.GetString(bytes);
    }

    internal static void ThrowIfFailedQueryData(nint handle)
    {
        if (FptrLibrary.Instance.QueryData(handle) != 0)
            throw new FptrException(handle);
    }

    internal static void ThrowIfFailedFnQueryData(nint handle)
    {
        if (FptrLibrary.Instance.FnQueryData(handle) != 0)
            throw new FptrException(handle);
    }

    internal static void ThrowIfFailedUtilFormTLV(nint handle)
    {
        if (FptrLibrary.Instance.UtilFormTLV(handle) != 0)
            throw new FptrException(handle);
    }

    public static void ThrowIfNot(bool condition, nint handle)
    {
        if (!condition) 
            throw new FptrException(handle);
    }

    public static void ThrowIfFailed(FptrAction func, nint handle)
    {
        if (func(handle) == -1) 
            throw new FptrException(handle);
    }
}
