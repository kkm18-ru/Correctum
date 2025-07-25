using RetailCorrector.Data;
using System.Windows.Controls;

namespace RetailCorrector.Parsers
{
    internal interface IParserPresenter<TP, TV>
        where TP : class, IParserPresenter<TP, TV>
        where TV : Control, IParserView<TV, TP>, new()
    {
        public abstract static string DisplayName { get; }
        public static TV ShowView() => new();

        IAsyncEnumerable<Receipt> Parse(CancellationToken cancellationToken);
    }
}
