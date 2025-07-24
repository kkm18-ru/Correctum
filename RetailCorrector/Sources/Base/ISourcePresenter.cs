using RetailCorrector.Data;
using System.Windows.Controls;

namespace RetailCorrector.Sources
{
    internal interface ISourcePresenter<TP, TV>
        where TP : class, ISourcePresenter<TP, TV>
        where TV : Control, ISourceView<TV, TP>, new()
    {
        public static TV ShowView() => new();

        IAsyncEnumerable<Receipt> Parse(CancellationToken cancellationToken);
    }
}
