namespace Correctum;

public class RangeProgress<T> : IProgress<(T? Current, T? Maximum)>
    where T : struct, IComparable<T>
{
    private readonly Action<T> _currentHandler;
    private readonly Action<T> _maximumHandler;
    private readonly SynchronizationContext _synchronizationContext;
    private readonly SendOrPostCallback _invokeHandlers;
    private static readonly SynchronizationContext _defaultContext = new();

    public RangeProgress(Action<T> currentHandler, Action<T> maximumHandler)
    {
        _currentHandler = currentHandler;
        _maximumHandler = maximumHandler;
        _synchronizationContext = SynchronizationContext.Current ?? _defaultContext;
        _invokeHandlers = new(InvokeHandlers);
    }

    public void Report((T? Current, T? Maximum) value) =>
        _synchronizationContext.Post(_invokeHandlers, value);

    public void ReportCurrent(T value) => 
        Report(((T? Current, T? Maximum))(value, null));
    public void ReportMaximum(T value) => 
        Report(((T? Current, T? Maximum))(default(T), value));

    private void InvokeHandlers(object? state)
    {
        var (current, maximum) = ((T? Current, T? Maximum))state!;
        if (current is not null) _currentHandler?.Invoke((T)current);
        if (maximum is not null) _maximumHandler?.Invoke((T)maximum);
    }
}
