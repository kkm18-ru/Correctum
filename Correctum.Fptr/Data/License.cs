namespace Correctum.Fptr.Data;

public readonly struct License
{
    public bool Entered { get; }
    public DateTime ValidFrom { get; }
    public DateTime ValidUntil { get; }

    internal License(nint handle)
    {
        Entered = FptrLibrary.Instance.GetParamBool(handle, 65611);
        ValidFrom = FptrLibrary.Instance.GetParamDateTime(handle, 65824);
        ValidUntil = FptrLibrary.Instance.GetParamDateTime(handle, 65825);
    }
}
