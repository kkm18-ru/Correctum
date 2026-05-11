namespace Correctum.Fptr.Functions;

public abstract class BaseFunctions(nint handle) : IDisposable
{
    protected bool disposed = false;
    protected nint handle = handle;
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed) return;
        handle = nint.Zero;
        disposed = true;
    }
}
