using Correctum.Utils.Extensions;

namespace Correctum.Tests.Utils.Extensions;

[TestFixture]
public class DateOnlyExtensions
{

    [Test]
    public void DiffDaysTest()
    {
        var less = new DateOnly(2020, 1, 1);
        var more = new DateOnly(2020, 2, 1);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(more - less, Is.EqualTo(TimeSpan.FromDays(31)));
            Assert.That(less - more, Is.EqualTo(TimeSpan.FromDays(-31)));
        }
    }
}
