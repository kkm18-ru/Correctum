namespace RetailCorrector.Utils;

public sealed class VisualProgress : IProgress<(int, int)>
{
    private readonly SynchronizationContext _synchronizationContext;
    private readonly SendOrPostCallback _invokeHandlers;
    
    public event EventHandler<int>? CurrentValueChanged;
    public event EventHandler<int>? MaxValueChanged;

    public VisualProgress()
    {
         _synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
         _invokeHandlers = InvokeHandlers;
    }

    void IProgress<(int, int)>.Report((int, int) value)
    {
        ReportCurrValue(value.Item1);
        ReportMaxValue(value.Item2);
    }

    public void ReportCurrValue(int value)
    {
        var progressChanged = CurrentValueChanged;
        if (progressChanged == null) return;
        _synchronizationContext.Post(_invokeHandlers, (value, ValueTypeVisualProgress.Current));
    }

    public void ReportMaxValue(int value)
    {
        ReportCurrValue(0);
        var progressChanged = MaxValueChanged;
        if (progressChanged == null) return;
        _synchronizationContext.Post(_invokeHandlers, (value, ValueTypeVisualProgress.Max));
    }
    
    private void InvokeHandlers(object? state)
    {
        ArgumentNullException.ThrowIfNull(state);
        var e = ((int, ValueTypeVisualProgress)) state;
        var progressChanged = e.Item2 switch
        {
            ValueTypeVisualProgress.Current => CurrentValueChanged,
            ValueTypeVisualProgress.Max => MaxValueChanged,
            _ => null
        };
        progressChanged?.Invoke(this, e.Item1);
    }
}

file enum ValueTypeVisualProgress { Current, Max }