using System.Windows.Controls;

namespace RetailCorrector.Sources
{
    internal interface ISourceView<TV, TP>
        where TV : Control, ISourceView<TV, TP>, new()
        where TP : class, ISourcePresenter<TP, TV>
    {
        TP Presenter { get; }
    }
}
