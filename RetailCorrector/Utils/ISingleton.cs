namespace RetailCorrector.Utils
{
    internal interface ISingleton<T>
        where T : class, ISingleton<T>
    {
        abstract static T Instance { get; }
    }
}
