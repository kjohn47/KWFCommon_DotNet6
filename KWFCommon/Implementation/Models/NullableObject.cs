namespace KWFCommon.Implementation.Models
{
    using KWFCommon.Abstractions.Models;

    public sealed class NullableObject<TObject> : INullableObject<TObject>
        where TObject : class
    {
        private NullableObject()
        {
            Value = null!;
            HasValue = false;
        }

        private NullableObject(TObject value)
        {
            Value = value;
            HasValue = true;
        }

        public TObject Value { get; init; }
        public bool HasValue { get; init; }

        public static NullableObject<TObject> EmptyResult()
        {
            return new NullableObject<TObject>();
        }

        public static NullableObject<TObject> FromResult(TObject result)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new NullableObject<TObject>(result);
        }
    }
}
