using Correctum.ViewModels.Controls;
using System.Windows.Controls;

namespace Correctum.Views.Controls;

public sealed class MultiProgressBar : StackPanel
{
    public MultiProgress Progress { get; private set; }

    public MultiProgressBar()
    {
        DataContext = Progress = new MultiProgress();
        Progress.BusyIndicatorCreated += BusyIndicatorCreating;
        Progress.LastStageTextChanged += LastStageTextChanging;
        Progress.StageCreated += StageCreating;
        Progress.LastStageRemoved += LastStageRemoving;
        Progress.LastStageDecremented += LastStageDecrementing;
        Progress.LastStageIncremented += LastStageIncrementing;
        Progress.AllStagesRemoved += Children.Clear;
    }


    private void BusyIndicatorCreating(string text)
    {
        Children.Add(new TextBlock { Text = text });
        Children.Add(new ProgressBar { Height = 5, IsIndeterminate = true });
        Children.Add(new Separator { Height = 5 });
    }

    private void LastStageRemoving()
    {
        for (var _ = 0; _ < 3; _++)
            Children.RemoveAt(Children.Count - 1);
    }

    private void StageCreating(int maximum)
    {
        Children.Add(new TextBlock());
        Children.Add(new ProgressBar { Height = 5, Maximum = maximum, Value = 0 });
        Children.Add(new Separator { Height = 5 });
    }

    private void LastStageIncrementing() => ((ProgressBar)Children[^2]).Value++;
    private void LastStageDecrementing() => ((ProgressBar)Children[^2]).Value--;
    private void LastStageTextChanging(string text) => ((TextBlock)Children[^3]).Text = text;
}
