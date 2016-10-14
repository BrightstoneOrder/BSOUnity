namespace Brightstone
{
    public enum StreamError
    {
        SE_INVALID_CONTEXT,
        SE_INVALID_OBJECT,
        SE_INVALID_OBJECT_TYPE,
        SE_INVALID_PROPERTY_TYPE,
        SE_UNEXPECTED_TOKEN
    }

    public class StreamErrorBitfield : Bitfield<StreamError> { }
}