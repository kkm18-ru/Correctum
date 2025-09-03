namespace RetailCorrector.Exports
{
    internal interface IExport
    {
        string Name { get; }
        ValueTask<string> Export();
    }
}
