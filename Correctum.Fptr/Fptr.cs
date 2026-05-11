using Correctum.Fptr.Enums;
using Correctum.Fptr.Functions;

namespace Correctum.Fptr;

public sealed class Fptr : IDisposable
{
    public Session Session { get; private set; }
    public FiscalDrive FiscalDrive { get; private set; }
    public Cashbox Cashbox { get; private set; }
    public Driver Driver { get; private set; }
    public Receipt Receipt { get; private set; }

    public FiscalDocumentFormat FiscalFormat
    {
        get
        {
            var drive = (uint)FiscalDrive.FiscalFormat;
            var kkt = (uint)Cashbox.FiscalFormat;
            return (FiscalDocumentFormat)Math.Min(drive, kkt);
        }
    }

    private nint handle;
    private bool disposed;

    internal Fptr()
    {
        disposed = false;
        handle = nint.Zero;
        FptrLibrary.Instance.Create(ref handle);
        Session = new(handle);
        FiscalDrive = new(handle);
        Cashbox = new(handle);
        Driver = new(handle);
        Receipt = new(handle);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Fptr() => Dispose(false);
    private void Dispose(bool disposing)
    {
        if (disposed) return;
        DisposeFunctions();
        if (disposing)
            FptrLibrary.Instance.Destroy(ref handle);
        SetNullFunctions();
        disposed = true;
    }

    private void DisposeFunctions()
    {
        Session.Dispose();
        FiscalDrive.Dispose();
        Cashbox.Dispose();
        Driver.Dispose();
        Receipt.Dispose();
    }

    private void SetNullFunctions()
    {
        Session = null!;
        FiscalDrive = null!;
        Cashbox = null!;
        Driver = null!;
        Receipt = null!;
    }
}
