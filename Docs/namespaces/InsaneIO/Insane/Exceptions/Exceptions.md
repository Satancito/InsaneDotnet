# InsaneIO.Insane.Exceptions

Parent namespace: [InsaneIO.Insane](../Insane.md)

This namespace exposes exceptions that are intended to be meaningful to library consumers.

## Public types

- `DeserializeException`

## DeserializeException

`DeserializeException` is the package's canonical failure type for invalid JSON payloads and invalid dynamic deserialization input.

### What it represents

The exception means the input payload could not be accepted for the expected target type. Typical causes include:

- missing `TypeIdentifier`
- wrong `TypeIdentifier`
- missing required properties
- invalid enum values
- invalid nested JSON for dependent serializable types

### Constructor

```csharp
public DeserializeException(Type type, string content = "")
```

- `type` is the target type or contract that deserialization expected.
- `content` is the original serialized input, preserved for the error message.

### Message format

The current implementation formats the message as:

```text
Invalid content "<content>" to deserialize for the type "<type name>".
```

### Usage guidance

If you build higher-level APIs on top of this package, catch `DeserializeException` when you want to distinguish:

- bad payload shape
- unknown type identifier
- contract/type mismatch

from operational failures such as I/O or network issues.
