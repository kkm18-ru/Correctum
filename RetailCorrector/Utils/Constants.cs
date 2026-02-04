using System.Globalization;

namespace RetailCorrector.Utils;

public static class Constants
{
    public static readonly DateTime OfdBeginDate = new(2016, 6, 1, new GregorianCalendar());

    public const string DocsUrl = "https://ornaras.github.io/RetailCorrector/";
    public const string RoadmapUrl = "https://github.com/ornaras/RetailCorrector/milestones";
    public const string ChangelogUrl = "https://github.com/ornaras/RetailCorrector/blob/dev/CHANGELOG.md";
}