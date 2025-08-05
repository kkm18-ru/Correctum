using RetailCorrector.Data;

namespace RetailCorrector.Parsers
{
    internal abstract class ParserPresenter<TP, TV> : ParserPresenter
        where TP : ParserPresenter<TP, TV>, new()
        where TV : ParserView<TV, TP>, new()
    {
        public static TV ShowView() => new();
    }

    internal abstract class ParserPresenter
    {
        public abstract IAsyncEnumerable<Receipt> Parse(CancellationToken cancellationToken);
    }
}
