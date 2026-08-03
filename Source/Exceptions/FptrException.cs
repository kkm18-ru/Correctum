using Correctum.Utils.Integrations;

namespace Correctum.Exceptions;

public sealed class FptrException : Exception
{
    public int Code { get; }

    internal FptrException(FptrHandle fptr) : base(fptr.ErrorDescription)
    {
        Code = fptr.ErrorCode;
    }
}
