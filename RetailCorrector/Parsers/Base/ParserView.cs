using System.Windows.Controls;

namespace RetailCorrector.Parsers
{
    internal abstract class ParserView<TV, TP> : UserControl
        where TV : ParserView<TV, TP>, new()
        where TP : ParserPresenter<TP, TV>, new()
    {
        TP Presenter { get; }
    }
}
