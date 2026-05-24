namespace Correctum.ViewModels.Controls;

public sealed class MultiProgress
{
    public event Action<string>? BusyIndicatorCreated;
    public event Action<string>? LastStageTextChanged;
    public event Action<int>? StageCreated;
    public event Action? LastStageRemoved;
    public event Action? LastStageDecremented;
    public event Action? LastStageIncremented;
    public event Action? AllStagesRemoved;

    public IDisposable Busy(string text) => new MultiProgressBusyIndicator(this, text);

    internal void OnBusyIndicatorCreated(string text) => BusyIndicatorCreated?.Invoke(text);
    public void RemoveStage() => LastStageRemoved?.Invoke();
    public void RemoveAllStages() => AllStagesRemoved?.Invoke();
    public void UpdateStageText(string text) => LastStageTextChanged?.Invoke(text);
    public void CreateStage(int maximum) => StageCreated?.Invoke(maximum);
    public void Decrement() => LastStageDecremented?.Invoke();
    public void Increment() => LastStageIncremented?.Invoke();
}

public readonly struct MultiProgressBusyIndicator : IDisposable
{
    private readonly MultiProgress parent;

    internal MultiProgressBusyIndicator(MultiProgress parent, string text)
    {
        this.parent = parent;
        parent.OnBusyIndicatorCreated(text);
    }

    public void Dispose() => parent.RemoveStage();
}
