using System.Windows.Controls;

namespace RetailCorrector.Sources.OfdRu
{
    internal class OfdRuView : UserControl, ISourceView<OfdRuView, OfdRuPresenter>
    {
        public OfdRuPresenter Presenter { get; } = new OfdRuPresenter();

        public OfdRuView()
        {

        }
    }
}
