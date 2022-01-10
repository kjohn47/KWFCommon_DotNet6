namespace KWFCommon.Abstractions.Models
{
    public interface INullableObject<TObject>
        where TObject : class
    {
        TObject Value { get;}
        bool HasValue { get; }
    }
}
