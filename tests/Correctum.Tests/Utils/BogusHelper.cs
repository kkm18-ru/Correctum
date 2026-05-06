using Bogus;

namespace Correctum.Tests.Utils;

internal static class BogusHelper
{
    public static Faker SharedFaker => field ??= new Faker("ru");
}
