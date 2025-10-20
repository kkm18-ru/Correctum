using System.Windows.Controls;

namespace RetailCorrector.Exports
{
    internal interface IExport
    {
        string Name { get; }
        string? FileFilter { get; }
        ValueTask Export(string? path);
        void InitVisual(Grid content) { }
    }
}
