namespace Correctum.ViewModels.Controls;

public sealed class MultiProgress
{
    public event Action<string>? BusyIndicatorCreated;
    public event Action<string>? LastStageTextChanged;
    public event Action<int>? StageCreated;
    public event Action? LastStageRemoved;
    public event Action? LastStageIncremented;
    public event Action? AllStagesRemoved;

    internal void OnBusyIndicatorCreated(string text) => BusyIndicatorCreated?.Invoke(text);
    internal void RemoveStage() => LastStageRemoved?.Invoke();
    internal void RemoveAllStages() => AllStagesRemoved?.Invoke();
    internal void UpdateStageText(string text) => LastStageTextChanged?.Invoke(text);
    internal void CreateStage(int maximum) => StageCreated?.Invoke(maximum);
    internal void Increment() => LastStageIncremented?.Invoke();

    public IDisposable Busy(string text) => new MultiProgressBusyIndicator(this, text);
    public IEnumerable<T> Stage<T>(IEnumerable<T> source, string template)
    {
        CreateStage(source.Count());
        foreach(var item in source)
        {
            UpdateStageText(string.Format(template, item));
            yield return item;
            Increment();
        }
        RemoveStage();
    }
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