namespace Correctum.ViewModels.Controls;

public sealed class MultiProgress
{
    public event Action<int>? StageAdded;
    public event Action? StageRemoved;
    public event Action? AllStagesRemoved;
    public event Action<int>? Incremented;
    public event Action<int>? Decremented;
    public event Action<int, string>? StageTextUpdated;

    public void AddStage(int maximum) => StageAdded?.Invoke(maximum);
    public void SetStageText(int index, string text) => StageTextUpdated?.Invoke(index, text);
    public void Increment(int index) => Incremented?.Invoke(index);
    public void Decrement(int index) => Decremented?.Invoke(index);
    public void RemoveStage() => StageRemoved?.Invoke();
    public void RemoveAllStages() => AllStagesRemoved?.Invoke();
}
