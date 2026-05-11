namespace Correctum.Fptr.Functions;

public sealed class Driver : BaseFunctions
{
    internal Driver(nint handle) : base(handle) { }

    public bool ShowProperties(nint hWnd) // todo
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        return FptrLibrary.Instance.ShowProperties(handle, 0, hWnd) == 0;
    }
}
