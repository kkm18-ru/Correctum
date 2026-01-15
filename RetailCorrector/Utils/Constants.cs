using System.Globalization;

namespace RetailCorrector.Utils;

public static class Constants
{
    public static readonly DateTime OfdBeginDate = new(2016, 6, 1, new GregorianCalendar());

    public const string DocsUrl = "https://retailcorrector.readthedocs.io/";
    public const string RoadmapUrl = "https://gitlab.com/ornaras/retailcorrector/-/milestones";
    public const string ChangelogUrl = "https://gitlab.com/ornaras/retailcorrector/-/releases";
}