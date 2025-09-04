using RetailCorrector.Data;

namespace RetailCorrector.Parsers
{
    internal abstract class ParserBase<TP, TV> : IParser
        where TP : ParserBase<TP, TV>, new()
        where TV : ParserView<TV, TP>, new()
    {
        public static TV ShowView() => new();
        public abstract IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, CancellationToken cancellationToken);
    }

    internal interface IParser
    {
        IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, CancellationToken cancellationToken);
    }
}
