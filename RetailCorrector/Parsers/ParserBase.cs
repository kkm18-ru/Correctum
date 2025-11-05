using RetailCorrector.Data;
using RetailCorrector.UI.Windows;
using RetailCorrector.Utils;

namespace RetailCorrector.Parsers
{
    internal abstract class ParserBase<TP, TV> : IParser, ISingleton<TP>
        where TP : ParserBase<TP, TV>, ISingleton<TP>, new()
        where TV : ParserView<TV, TP>, new()
    {
        private static TP _instance = null!;
        public static TP Instance => _instance ??= new TP();

        public abstract string Name { get; }
        protected ParseInfo parseInfo;
        public abstract ParseInfo GenerateInfo();

        public void Show() => new ParserWindow(new TV()).ShowDialog();
        public abstract IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, CancellationToken cancellationToken);
    }

    internal interface IParser
    {
        string Name { get; }
        void Show();
        ParseInfo GenerateInfo();
        IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, CancellationToken cancellationToken);
    }
}
