using System.Numerics;

namespace RetailCorrector.Utils;

public class BoundedProgress<T> where T : struct, IUnsignedNumber<T>
{
    public event Action<T>? MaximumChanged;
    public event Action? ValueCleared;
    public event Action? ValueIncremented;

    private readonly SynchronizationContext _synchronizationContext;

    private readonly SendOrPostCallback _invokeClearHandlers;
    private readonly SendOrPostCallback _invokeMaximumHandlers;
    private readonly SendOrPostCallback _invokeIncrementHandlers;

    public BoundedProgress()
    {
        _synchronizationContext = SynchronizationContext.Current ?? new();
        _invokeClearHandlers = new SendOrPostCallback(InvokeClearHandlers);
        _invokeMaximumHandlers = new SendOrPostCallback(InvokeMaximumHandlers);
        _invokeIncrementHandlers = new SendOrPostCallback(InvokeIncrementHandlers);
    }

    public void ReportIncrement()
    {
        if (ValueIncremented is not null)
            _synchronizationContext.Post(_invokeIncrementHandlers, null);
    }
    public void ReportMaximum(T value)
    {
        if (MaximumChanged is null || ValueCleared is null) return;
        _synchronizationContext.Post(_invokeMaximumHandlers, value);
        _synchronizationContext.Post(_invokeClearHandlers, null);
    }
    private void InvokeClearHandlers(object? _) => ValueCleared?.Invoke();
    private void InvokeMaximumHandlers(object? state) => MaximumChanged?.Invoke((T)state!);
    private void InvokeIncrementHandlers(object? _) => ValueIncremented?.Invoke();
}
