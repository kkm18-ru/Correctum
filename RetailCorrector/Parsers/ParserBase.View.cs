using System.Windows.Controls;
using RetailCorrector.Utils;

namespace RetailCorrector.Parsers
{
    internal abstract class ParserView<TV, TP> : UserControl
        where TV : ParserView<TV, TP>, new()
        where TP : ParserBase<TP, TV>, ISingleton<TP>, new()
    {
        public TP Presenter => (TP)DataContext;

        protected ParserView()
        {
            Margin = new System.Windows.Thickness(0, 0, 0, 5);
            DataContext = TP.Instance;
        }
    }
}
