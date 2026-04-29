namespace InsaneIO.Insane.Extensions;

public static class EnumExtensions
{
    public static short ShortValue<TEnum>(this TEnum value) where TEnum : Enum
    {
        return (short)Enum.ToObject(value.GetType(), value);
    }

    public static int IntValue<TEnum>(this TEnum value) where TEnum : Enum
    {
        return (int)Enum.ToObject(value.GetType(), value);
    }

    public static long LongValue<TEnum>(this TEnum value) where TEnum : Enum
    {
        return (long)Enum.ToObject(value.GetType(), value);
    }

    public static ushort UShortValue<TEnum>(this TEnum value) where TEnum : Enum
    {
        return (ushort)Enum.ToObject(value.GetType(), value);
    }

    public static uint UIntValue<TEnum>(this TEnum value) where TEnum : Enum
    {
        return (uint)Enum.ToObject(value.GetType(), value);
    }

    public static ulong ULongValue<TEnum>(this TEnum value) where TEnum : Enum
    {
        return (ulong)Enum.ToObject(value.GetType(), value);
    }

    public static byte ByteValue<TEnum>(this TEnum value) where TEnum : Enum
    {
        return (byte)Enum.ToObject(value.GetType(), value);
    }

    public static sbyte SByteValue<TEnum>(this TEnum value) where TEnum : Enum
    {
        return (sbyte)Enum.ToObject(value.GetType(), value);
    }

    public static TNumber NumberValue<TNumber>(this Enum value) where TNumber : IConvertible
    {
        return (TNumber)Enum.ToObject(value.GetType(), value);
    }

    public static TEnum ParseEnum<TEnum, TNumber>(this TNumber value) where TNumber : IConvertible where TEnum: Enum
    {
        return (TEnum)Enum.ToObject(typeof(TEnum), value);
    }
}
