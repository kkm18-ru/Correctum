using System.Windows.Controls;

namespace RetailCorrector.Exports
{
    internal interface IExport
    {
        string? FileFilter { get; }
        ValueTask Export(string? path);
        void InitVisual(Grid content) { }
    }
}
