using Correctum.Abstractions;
using Correctum.Utils.Integrations;

namespace Correctum.ViewModels.Windows;

internal sealed class ExchangeOfdWindowViewModel : BindableBase
{
    public double CurrentProgress { get; private set => SetField(ref field, value); }
    public double CurrentSpeed { get; private set => SetField(ref field, value); }
    public uint FirstUnsentDocument { get; private set => SetField(ref field, value); }
    public TimeSpan RemainingTime { get; private set => SetField(ref field, value); }
    public uint RemainingCount { get; private set => SetField(ref field, value); }
    public uint InitializeUnsentCount { get; init; }
    public DateTime InitializeDateTime { get; init; }

    public event Action? Completed;

    public ExchangeOfdWindowViewModel(FptrHandle fptr)
    {
        InitializeDateTime = DateTime.UtcNow;
        InitializeUnsentCount = fptr.GetExchangeOfdStatus()!.Value.UnsentCount;
        var timer = new System.Timers.Timer(TimeSpan.FromMilliseconds(250));
        timer.Elapsed += (_, _) =>
        {
            var status = fptr.GetExchangeOfdStatus()!.Value;
            FirstUnsentDocument = status.FirstUnsentNumber;
            RemainingCount = status.UnsentCount;
            var sentCount = InitializeUnsentCount - RemainingCount;
            CurrentProgress = sentCount * 100d / InitializeUnsentCount;
            var now = DateTime.UtcNow;
            var elapsedTime = now - InitializeDateTime;
            CurrentSpeed = sentCount / elapsedTime.TotalSeconds;
            RemainingTime = TimeSpan.FromSeconds(CurrentSpeed > 0 ? RemainingCount / CurrentSpeed : 0);
            if (CurrentProgress >= 100)
            {
                Completed?.Invoke();
                timer.Stop();
            }
        };
        timer.Start();
    }
}
