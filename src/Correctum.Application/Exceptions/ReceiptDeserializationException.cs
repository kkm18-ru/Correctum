namespace Correctum.Exceptions;

public sealed class ReceiptDeserializationException(string message, Exception innerException) 
    : Exception(message, innerException);
