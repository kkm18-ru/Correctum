using RetailCorrector.Utils;

namespace RetailCorrector.Tests.Utils;

public sealed class BoundedProgressTests
{
    [Test]
    public void ReportMaximum_Should_ResetValue_And_SetMaximum()
    {
        var prevContext = SynchronizationContext.Current;
        try
        {
            var context = new TestSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(context);
            uint maximum = 321, value = 123;
            var progress = new BoundedProgress<uint>();
            progress.MaximumChanged += v => maximum = v;
            progress.ValueCleared += () => value = 0;
            using (Assert.EnterMultipleScope())
            {
                progress.ReportMaximum(100);
                context.ExecuteAll();
                Assert.That(value, Is.Zero);
                Assert.That(maximum, Is.EqualTo(100));
            }
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(prevContext);
        }
    }

    [Test]
    public void ReportIncrement_Should_IncrementValue()
    {
        var prevContext = SynchronizationContext.Current;
        try
        {
            var context = new TestSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(context);
            uint value = 0;
            var progress = new BoundedProgress<uint>();
            progress.ValueIncremented += () => value++;
            progress.ReportIncrement();
            context.ExecuteAll();
            Assert.That(value, Is.EqualTo(1));
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(prevContext);
        }
    }

    [Test]
    public void ReportIncrement_Should_ThrowException()
    {
        var prevContext = SynchronizationContext.Current;
        try
        {
            var context = new TestSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(context);
            var progress = new BoundedProgress<uint>();
            using (Assert.EnterMultipleScope())
            {
                var ex = Assert.Throws<ArgumentNullException>(progress.ReportIncrement);
                Assert.That(ex!.ParamName, Is.EqualTo(nameof(BoundedProgress<>.ValueIncremented)));
            }
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(prevContext);
        }
    }

    [Test]
    public void ReportMaximum_Should_ThrowException_About_ValueCleared()
    {
        var prevContext = SynchronizationContext.Current;
        try
        {
            var context = new TestSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(context);
            uint maximum = 321;
            var progress = new BoundedProgress<uint>();
            progress.MaximumChanged += v => maximum = v;
            using (Assert.EnterMultipleScope())
            {
                var ex = Assert.Throws<ArgumentNullException>(() =>
                {
                    progress.ReportMaximum(100);
                    context.ExecuteAll();
                });
                Assert.That(ex!.ParamName, Is.EqualTo(nameof(BoundedProgress<>.ValueCleared)));
                Assert.That(maximum, Is.EqualTo(321));
            }
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(prevContext);
        }
    }

    [Test]
    public void ReportMaximum_Should_ThrowException_About_MaximumChanged()
    {
        var prevContext = SynchronizationContext.Current;
        try
        {
            var context = new TestSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(context);
            uint value = 123;
            var progress = new BoundedProgress<uint>();
            progress.ValueCleared += () => value = 0;
            using (Assert.EnterMultipleScope())
            {
                var ex = Assert.Throws<ArgumentNullException>(() =>
                {
                    progress.ReportMaximum(100);
                    context.ExecuteAll();
                });
                Assert.That(ex!.ParamName, Is.EqualTo(nameof(BoundedProgress<>.MaximumChanged)));
                Assert.That(value, Is.EqualTo(123));
            }
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(prevContext);
        }
    }
}
