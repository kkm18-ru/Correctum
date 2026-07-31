namespace Correctum.Exceptions;

public sealed class ModelValidationException
    (ModelValidationException.ModelType type, int modelId, string message) : 
    Exception(message)
{
    public enum ModelType { Receipt, Position }

    public ModelType Type { get; } = type;
    public int ModelId { get; } = modelId;
}
