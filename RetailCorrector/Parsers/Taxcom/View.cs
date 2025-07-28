using System.Windows.Controls;

namespace RetailCorrector.Parsers.Taxcom
{
    internal sealed class TaxcomView : UserControl, IParserView<TaxcomView, TaxcomPresenter>
    {
        public TaxcomPresenter Presenter { get; } = new();
    }
}
