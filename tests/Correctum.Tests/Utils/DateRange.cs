using Correctum.Utils.Extensions;

namespace Correctum.Tests.Utils;

[TestFixture]
public class DateRange
{
    [Test]
    public void EnumerableTest()
    {
        DateOnly[] array = [
            new DateOnly(2020, 1, 1),
            new DateOnly(2020, 1, 2),
            new DateOnly(2020, 1, 3),
            new DateOnly(2020, 1, 4),
            new DateOnly(2020, 1, 5),
        ];
        var daterange = new Correctum.Utils.DateRange
        {
            BeginDate = array[0],
            EndDate = array[^1]
        };
        var i = 0;
        using (Assert.EnterMultipleScope())
        {
            foreach (var date in daterange)
            {
                Assert.That(date, Is.EqualTo(array[i]));
                i++;
            }
        }
    }

    [Test]
    public void SetDateBeforeOfdCreatedTest()
    {
        var daterange = new Correctum.Utils.DateRange();
        var beforeOfdCreated = new DateOnly(2015, 1, 1);
        daterange.BeginDate = beforeOfdCreated;
        Assert.That(daterange.BeginDate, Is.Not.EqualTo(beforeOfdCreated));
    }

    [Test]
    public void SetTomorrowDateTest()
    {
        var daterange = new Correctum.Utils.DateRange();
        var tomorrow = DateOnly.Today.AddDays(1);
        daterange.EndDate = tomorrow;
        Assert.That(daterange.EndDate, Is.Not.EqualTo(tomorrow));
    }

    [Test]
    public void SetDateOutsideRangeTest()
    {
        var begin = new DateOnly(2020, 1, 1);
        var end = new DateOnly(2022, 12, 31);

        var daterange = new Correctum.Utils.DateRange
        { 
            BeginDate = begin, 
            EndDate = end 
        };

        daterange.BeginDate = end.AddDays(1);
        daterange.EndDate = begin.AddDays(-1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(daterange.BeginDate, Is.EqualTo(begin));
            Assert.That(daterange.EndDate, Is.EqualTo(end));
        }
    }

    [Test]
    public void SkipRepeatedSetterTest()
    {
        var begin = new DateOnly(2020, 1, 1);
        var end = new DateOnly(2022, 12, 31);

        var daterange = new Correctum.Utils.DateRange
        { 
            BeginDate = begin, 
            EndDate = end 
        };

        bool repeatedBegin = false, repeatedEnd = false;
        daterange.PropertyChanged += (_, e) =>
        {
            if(e.PropertyName == nameof(daterange.BeginDate)) repeatedBegin = true;
            if(e.PropertyName == nameof(daterange.EndDate)) repeatedEnd = true;
        };

        daterange.BeginDate = begin;
        daterange.EndDate = end;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(repeatedBegin, Is.False);
            Assert.That(repeatedEnd, Is.False);
        }
    }
}
