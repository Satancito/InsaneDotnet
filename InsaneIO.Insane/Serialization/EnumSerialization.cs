using InsaneIO.Insane.Exceptions;
using System.Text.Json.Nodes;

namespace InsaneIO.Insane.Serialization;

public static class EnumSerialization
{
    public static TEnum ReadEnumValue<TEnum>(JsonNode jsonNode, string propertyName, Type ownerType, string json)
        where TEnum : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(jsonNode);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentNullException.ThrowIfNull(ownerType);

        JsonNode propertyNode = jsonNode[propertyName] ?? throw new DeserializeException(ownerType, json);

        try
        {
            if (propertyNode is JsonValue jsonValue)
            {
                if (jsonValue.TryGetValue<int>(out int numericValue))
                {
                    TEnum enumValue = (TEnum)Enum.ToObject(typeof(TEnum), numericValue);
                    if (Enum.IsDefined(enumValue))
                    {
                        return enumValue;
                    }

                    throw new DeserializeException(ownerType, json);
                }

                if (jsonValue.TryGetValue<string>(out string? stringValue)
                    && !string.IsNullOrWhiteSpace(stringValue)
                    && Enum.TryParse(stringValue, ignoreCase: true, out TEnum parsedValue)
                    && Enum.IsDefined(parsedValue))
                {
                    return parsedValue;
                }
            }
        }
        catch (DeserializeException)
        {
            throw;
        }
        catch
        {
        }

        throw new DeserializeException(ownerType, json);
    }
}
