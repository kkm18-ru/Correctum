using RetailCorrector.Data;
using RetailCorrector.UI;

namespace RetailCorrector.Parsers
{
    internal abstract class ParserBase<TP, TV> : IParser
        where TP : ParserBase<TP, TV>, new()
        where TV : ParserView<TV, TP>, new()
    {
        protected ParseInfo parseInfo;
        public abstract ParseInfo GenerateInfo();

        public static void Show() => new ParserWindow(new TV()).ShowDialog();
        public abstract IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, CancellationToken cancellationToken);
    }

    internal abstract class ParserBase<T> : IParser
        where T : ParserBase<T>, new()
    {
        protected ParseInfo parseInfo;
        public abstract ParseInfo GenerateInfo();

        public static void Show() => new ParserWindow(new T()).ShowDialog();
        public abstract IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, CancellationToken cancellationToken);
    }

    internal interface IParser
    {
        ParseInfo GenerateInfo();
        IAsyncEnumerable<Receipt> Parse(IProgress<long> progress, CancellationToken cancellationToken);
    }
}
