namespace Correctum.Storage
{
    internal interface IDto<TSelf, TModel>
        where TSelf : IDto<TSelf, TModel>
        where TModel : new()
    {
        TModel ToModel();
        abstract static TSelf FromModel(TModel model);

        void ToBytes(Span<byte> bytes);
        abstract static TSelf FromBytes(ReadOnlySpan<byte> bytes);
    }
}
