using RetailCorrector.Data;

namespace RetailCorrector.Parsers
{
    internal abstract class ParserPresenter<TP, TV>
        where TP : ParserPresenter<TP, TV>, new()
        where TV : ParserView<TV, TP>, new()
    {
        public static TV ShowView() => new();

        public abstract IAsyncEnumerable<Receipt> Parse(CancellationToken cancellationToken);
    }
}
