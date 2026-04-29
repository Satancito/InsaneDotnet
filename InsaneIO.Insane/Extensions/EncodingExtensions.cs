namespace InsaneIO.Insane.Extensions;

public static class EncodingExtensions
{
    public static byte[] ToByteArrayUtf8(this string data)
    {
        return ToByteArray(data, Encoding.UTF8);
    }

    public static string ToStringUtf8(this byte[] utf8Bytes)
    {
        return ToString(utf8Bytes, Encoding.UTF8);
    }

    public static byte[] ToByteArray(this string data, Encoding encoding)
    {
        return encoding.GetBytes(data);
    }

    public static string ToString(this byte[] encodedBytes, Encoding encoding)
    {
        return encoding.GetString(encodedBytes);
    }
}
