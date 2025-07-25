using System.Windows.Controls;

namespace RetailCorrector.Parsers
{
    internal interface IParserView<TV, TP>
        where TV : Control, IParserView<TV, TP>, new()
        where TP : class, IParserPresenter<TP, TV>
    {
        TP Presenter { get; }
    }
}
