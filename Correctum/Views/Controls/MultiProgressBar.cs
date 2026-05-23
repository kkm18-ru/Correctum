using Correctum.ViewModels.Controls;
using System.Windows.Controls;

namespace Correctum.Views.Controls;

public sealed class MultiProgressBar : StackPanel
{
    public MultiProgress Progress { get; private set; }

    public MultiProgressBar()
    {
        DataContext = Progress = new MultiProgress();
        Progress.StageAdded += OnStageAdded;
        Progress.StageRemoved += OnStageRemoved;
        Progress.AllStagesRemoved += Children.Clear;
        Progress.Incremented += OnIncremented;
        Progress.Decremented += OnDecremented;
        Progress.StageTextUpdated += OnStageTextUpdated;
    }

    private void OnStageRemoved()
    {
        for (var _ = 0; _ < 3; _++)
            Children.RemoveAt(Children.Count - 1);
    }

    private void OnStageAdded(int maximum)
    {
        Children.Add(new TextBlock());
        Children.Add(new ProgressBar { Height = 5, Maximum = maximum, Value = 0 });
        Children.Add(new Separator { Height = 5 });
    }

    private void OnIncremented(int index) => ((ProgressBar)Children[index * 3 + 1]).Value++;
    private void OnDecremented(int index) => ((ProgressBar)Children[index * 3 + 1]).Value--;
    private void OnStageTextUpdated(int index, string text) => ((TextBlock)Children[index * 3]).Text = text;
}
