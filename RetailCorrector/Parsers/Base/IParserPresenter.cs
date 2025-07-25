using RetailCorrector.Data;
using System.Windows.Controls;

namespace RetailCorrector.Parsers
{
    internal interface IParserPresenter<TP, TV> : IParserPresenter
        where TP : class, IParserPresenter<TP, TV>
        where TV : Control, IParserView<TV, TP>, new()
    {
        public abstract static string DisplayName { get; }
        public abstract static TV ShowView();
    }

    internal interface IParserPresenter
    {
        IAsyncEnumerable<Receipt> Parse(CancellationToken cancellationToken);
    }
}
