using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Correctum.Utils;

public class DateRange : IEnumerable<DateOnly>, INotifyPropertyChanged
{
    public DateOnly BeginDate
    {
        get => MinValue.AddDays(_beginDays);
        set
        {
            if (!ValidDateByPeriod(value)) return;

            var days = (int)(value - MinValue).TotalDays;
            if (days == _beginDays || days > _endDays) return;

            _beginDays = days;
            OnPropertyChanged();
        }
    }

    public DateOnly EndDate
    {
        get => MinValue.AddDays(_endDays);
        set
        {
            if (!ValidDateByPeriod(value)) return;

            var days = (int)(value - MinValue).TotalDays;
            if (days == _endDays || days < _beginDays) return;

            _endDays = days;
            OnPropertyChanged();
        }
    }

    public DateTime BeginDateTime => BeginDate.ToDateTime(TimeOnly.MinValue);
    public DateTime EndDateTime => EndDate.ToDateTime(TimeOnly.MaxValue);

    public static DateOnly MinValue => DateOnly.OfdBeginDate;
    public static DateOnly MaxValue => DateOnly.Today;
    public static DateTime MinValueDateTime => DateOnly.OfdBeginDate.ToDateTime(TimeOnly.MinValue);
    public static DateTime MaxValueDateTime => DateOnly.Today.ToDateTime(TimeOnly.MaxValue);

    private int _beginDays;
    private int _endDays;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DateRange()
    {
        var maxDays = Math.Min(MaxValue.DayNumber - MinValue.DayNumber, ushort.MaxValue);
        _beginDays = _endDays = maxDays;
    }

    public DateRange(DateOnly begin, DateOnly end) : this() => SetRangeUnsafe(begin, end);
    public DateRange(DateTime begin, DateTime end) : 
        this(DateOnly.FromDateTime(begin), DateOnly.FromDateTime(end)) { }

    public int Count => _endDays - _beginDays + 1;

    public IEnumerator<DateOnly> GetEnumerator()
    {
        for (var d = BeginDate; d <= EndDate; d = d.AddDays(1))
            yield return d;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void SetRangeUnsafe(DateOnly begin, DateOnly end)
    {
        if (!ValidDateByPeriod(begin) || !ValidDateByPeriod(end)) return;
        if (begin > end) return;

        _beginDays = (int)(begin - MinValue).TotalDays;
        _endDays = (int)(end - MinValue).TotalDays;

        OnPropertyChanged(nameof(BeginDate));
        OnPropertyChanged(nameof(EndDate));
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static bool ValidDateByPeriod(DateOnly value) =>
        value >= MinValue && value <= MaxValue;
}
