using System.Windows.Controls;

namespace RetailCorrector.Parsers
{
    internal abstract class ParserView<TV, TP> : UserControl
        where TV : ParserView<TV, TP>, new()
        where TP : ParserBase<TP, TV>, new()
    {
        public TP Presenter => (TP)DataContext;

        protected ParserView()
        {
            DataContext = new TP();
        }
    }
}
