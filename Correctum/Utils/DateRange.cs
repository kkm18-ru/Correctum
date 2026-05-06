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
            if (days == _beginDays) return;
            if (days > _endDays) return;
            _beginDays = (ushort)days;
            _enumerator.SetBegin(value);
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
            if (days == _endDays) return;
            if (days < _beginDays) return;
            _endDays = (ushort)days;
            _enumerator.SetEnd(value);
            OnPropertyChanged();
        }
    }

    public static DateOnly MinValue { get; } = new(2016, 1, 1);
    public static DateOnly MaxValue => DateOnly.Today;

    public static DateTime MinValueDateTime { get; } = MinValue.ToDateTime(new());
    public static DateTime MaxValueDateTime => DateTime.Today;

    private ushort _beginDays;
    private ushort _endDays;
    private readonly DateRangeEnumerator _enumerator;

    public event PropertyChangedEventHandler? PropertyChanged;

    public DateRange()
    {
        _beginDays = _endDays = 
            (ushort)Math.Min(MaxValue.DayNumber - MinValue.DayNumber, ushort.MaxValue);
        _enumerator = new DateRangeEnumerator();
    }

    public int Count => _endDays - _beginDays + 1;

    public IEnumerator<DateOnly> GetEnumerator() => _enumerator;

    public void SetRangeUnsafe(DateOnly begin, DateOnly end)
    {
        if (!ValidDateByPeriod(begin) || !ValidDateByPeriod(end)) return;

        var beginDays = (begin - MinValue).TotalDays;
        var endDays = (end - MinValue).TotalDays;

        _beginDays = (ushort)beginDays;
        _endDays = (ushort)endDays;

        _enumerator.SetBegin(begin);
        _enumerator.SetEnd(end);

        OnPropertyChanged(nameof(BeginDate));
        OnPropertyChanged(nameof(EndDate));
    }

    public void SetRangeUnsafe(DateTime begin, DateTime end) =>
        SetRangeUnsafe(DateOnly.FromDateTime(begin), DateOnly.FromDateTime(end));

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static bool ValidDateByPeriod(DateOnly value) =>
        value >= MinValue && value <= MaxValue;
}

internal sealed class DateRangeEnumerator() : IEnumerator<DateOnly>
{
    public DateOnly Current => current;
    object IEnumerator.Current => Current;

    private DateOnly current;
    private DateOnly begin;
    private DateOnly end;

    public void SetBegin(DateOnly value)
    {
        begin = value.AddDays(-1);
        Reset();
    }

    public void SetEnd(DateOnly value)
    {
        end = value;
        Reset();
    }

    public void Reset() => current = begin;

    public bool MoveNext()
    {
        if(current.CompareTo(end) >= 0) return false;
        current = current.AddDays(1);
        return true;
    }

    public void Dispose() { }
}
