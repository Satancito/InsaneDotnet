# Cryptography

This document describes the `InsaneIO.Insane.Cryptography` namespace, the shared contracts in `InsaneIO.Insane.Cryptography.Abstractions`, the extension methods in `InsaneIO.Insane.Cryptography.Extensions`, and the supporting helpers in `InsaneIO.Insane.Extensions`.

The library provides:

- Encoders for converting bytes to text: Base64, Base32, and Hex
- General text and byte conversion helpers
- Plain hashes: MD5, SHA-1, SHA-256, SHA-384, and SHA-512
- HMAC with the same algorithms
- Key derivation with Scrypt and Argon2
- Symmetric encryption with AES-CBC
- Asymmetric encryption with RSA
- Serializable encryptors, hashers, encoders, key pairs, and TOTP configuration
- Blazor WebAssembly interop methods through `IJSRuntime`

Common namespaces:

```csharp
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Cryptography.Abstractions;
using InsaneIO.Insane.Cryptography.Extensions;
using InsaneIO.Insane.Extensions;
```

## Core Concepts

The API usually offers two usage styles.

The first style uses extension methods directly on `string` or `byte[]`:

```csharp
string hash = "Hello".ComputeHashEncoded(HexEncoder.DefaultInstance, HashAlgorithm.Sha256);
string encrypted = "Hello".EncryptAesCbcEncoded("12345678", Base64Encoder.DefaultInstance);
```

The second style uses configurable classes:

```csharp
var hasher = new ShaHasher
{
    HashAlgorithm = HashAlgorithm.Sha256,
    Encoder = HexEncoder.DefaultInstance
};

string hash = hasher.ComputeEncoded("Hello");
```

Overloads that receive `string` convert the text to UTF-8 bytes before operating. Overloads ending with `Encoded` return text encoded with an `IEncoder`; methods containing `FromEncoded` accept already encoded text, decode it with the provided `IEncoder`, and then verify or decrypt it.

In algorithm-specific extension methods, the naming order is `Action + Algorithm + Format`, for example `ComputeHashEncoded`, `VerifyHmacFromEncoded`, `ComputeScryptEncoded`, `DecryptAesCbcFromEncoded`, `EncryptAesCbcEncoded`, or `EncryptRsaEncoded`. Generic contracts keep algorithm-free names such as `EncryptEncoded`, `DecryptEncoded`, `ComputeEncoded`, and `VerifyEncoded`.

## Interfaces

The serialization and runtime contracts live in `InsaneIO.Insane.Cryptography.Abstractions`.

### IJsonSerializable

Common contract for JSON-convertible objects.

| Member | Type | Description |
| --- | --- | --- |
| `ToJsonObject()` | `JsonObject` | Builds the JSON representation of the object. |
| `Serialize(bool indented = false)` | `string` | Returns the JSON as text. |
| `GetIndentOptions(bool indented)` | `static JsonSerializerOptions` | Returns serializer options with or without indentation. |

### IEncoder

Common contract for byte-to-text encoders.

| Method | Return | Description |
| --- | --- | --- |
| `Encode(byte[] data)` | `string` | Encodes bytes to text. |
| `Encode(string data)` | `string` | Converts text to UTF-8 and encodes it. |
| `Decode(string data)` | `byte[]` | Decodes the text back to the original byte array. |
| `DeserializeDynamic(string json)` | `IEncoder` | Resolves the concrete type through `TypeIdentifier` and dispatches to the correct `Deserialize(string)` implementation. |

### IEncoderJsonSerializable

Extends `IJsonSerializable` and requires:

```csharp
public static abstract IEncoder Deserialize(string json);
```

### IEncryptor

Common contract for encryptors.

| Method | Return | Description |
| --- | --- | --- |
| `Encrypt(byte[] data)` | `byte[]` | Encrypts bytes and returns ciphertext bytes. |
| `Encrypt(string data)` | `byte[]` | Converts text to UTF-8 and encrypts the bytes. |
| `EncryptEncoded(byte[] data)` | `string` | Encrypts bytes and encodes the ciphertext with the configured encoder. |
| `EncryptEncoded(string data)` | `string` | Converts text to UTF-8, encrypts it, and encodes the result. |
| `Decrypt(byte[] data)` | `byte[]` | Decrypts ciphertext bytes. |
| `DecryptEncoded(string data)` | `byte[]` | Decodes the encrypted text with the configured encoder and then decrypts it. |
| `DeserializeDynamic(string json)` | `IEncryptor` | Resolves the concrete type through `TypeIdentifier` and dispatches to the correct `Deserialize(string)` implementation. |

### IEncryptorJsonSerializable

Extends `IJsonSerializable` and requires:

```csharp
public static abstract IEncryptor Deserialize(string json);
```

### IHasher

Common contract for hashers and key-derivation wrappers.

| Method | Return | Description |
| --- | --- | --- |
| `Compute(byte[] data)` | `byte[]` | Computes the hash or derived value as bytes. |
| `Compute(string data)` | `byte[]` | Converts text to UTF-8 and computes the result. |
| `ComputeEncoded(byte[] data)` | `string` | Computes the result and encodes it with the configured encoder. |
| `ComputeEncoded(string data)` | `string` | Converts text to UTF-8, computes the result, and encodes it. |
| `Verify(byte[] data, byte[] expected)` | `bool` | Recomputes and compares to the expected bytes. |
| `Verify(string data, byte[] expected)` | `bool` | Converts text to UTF-8, recomputes, and compares. |
| `VerifyEncoded(byte[] data, string expected)` | `bool` | Recomputes, encodes, and compares to the expected text. |
| `VerifyEncoded(string data, string expected)` | `bool` | Converts text to UTF-8, recomputes, encodes, and compares. |
| `DeserializeDynamic(string json)` | `IHasher` | Resolves the concrete type through `TypeIdentifier` and dispatches to the correct `Deserialize(string)` implementation. |

### IHasherJsonSerializable

Extends `IJsonSerializable` and requires:

```csharp
public static abstract IHasher Deserialize(string json);
```

### IRsaKeyPairJsonSerializable

Extends `IJsonSerializable` and requires:

```csharp
public static abstract RsaKeyPair Deserialize(string json);
```

## TypeIdentifier

Encoders, hashers, encryptors, and `RsaKeyPair` serialize a root `TypeIdentifier` field. Dynamic deserialization uses that identifier to resolve the concrete type.

This mechanism is powered by:

- `TypeIdentifierAttribute`
- `TypeIdentifierResolver`
- the interface-level `DeserializeDynamic(string json)` helpers

There is no `AssemblyName` fallback anymore. If `TypeIdentifier` is missing, invalid, or points to another type, deserialization throws `DeserializeException`.

Example root JSON shape:

```json
{
  "TypeIdentifier": "Insane-Cryptography-Base64Encoder"
}
```

## Custom Implementations

You can implement your own `IEncoder`, `IHasher`, or `IEncryptor` in another assembly and still participate in dynamic deserialization.

Requirements:

- implement the corresponding interface from `InsaneIO.Insane.Cryptography.Abstractions`
- add `TypeIdentifierAttribute` with a stable identifier
- include `TypeIdentifier` in `ToJsonObject()`
- implement `public static Deserialize(string json)` for the concrete type

### Custom IEncoder

```csharp
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Cryptography.Abstractions;
using InsaneIO.Insane.Cryptography.Attributes;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;

[TypeIdentifier("MyCompany-Cryptography-BinaryEncoder")]
public sealed class BinaryEncoder : IEncoder
{
    public string Encode(byte[] data)
    {
        return string.Concat(data.Select(value => Convert.ToString(value, 2)!.PadLeft(8, '0')));
    }

    public string Encode(string data)
    {
        return Encode(Encoding.UTF8.GetBytes(data));
    }

    public byte[] Decode(string data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.Length % 8 != 0 || data.Any(character => character != '0' && character != '1'))
        {
            throw new ArgumentException("Binary input must contain only 0 and 1 and be a multiple of 8 bits.", nameof(data));
        }

        return Enumerable.Range(0, data.Length / 8)
            .Select(index => Convert.ToByte(data.Substring(index * 8, 8), 2))
            .ToArray();
    }

    public JsonObject ToJsonObject() => new()
    {
        [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = "MyCompany-Cryptography-BinaryEncoder"
    };

    public string Serialize(bool indented = false) =>
        ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));

    public static IEncoder Deserialize(string json)
    {
        JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(BinaryEncoder), json);

        if (jsonNode["TypeIdentifier"]?.GetValue<string>() != "MyCompany-Cryptography-BinaryEncoder")
        {
            throw new DeserializeException(typeof(BinaryEncoder), json);
        }

        return new BinaryEncoder();
    }
}
```

### Custom IHasher

This example is intentionally simple. It demonstrates the API shape, but it is not a secure cryptographic hash.

```csharp
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Cryptography.Abstractions;
using InsaneIO.Insane.Cryptography.Attributes;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

[TypeIdentifier("MyCompany-Cryptography-XorHasher")]
public sealed class XorHasher : IHasher
{
    public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

    public byte[] Compute(byte[] data)
    {
        byte accumulator = 0;

        foreach (byte value in data)
        {
            accumulator ^= value;
        }

        return [accumulator];
    }

    public byte[] Compute(string data)
    {
        return Compute(Encoding.UTF8.GetBytes(data));
    }

    public string ComputeEncoded(byte[] data)
    {
        return Encoder.Encode(Compute(data));
    }

    public string ComputeEncoded(string data)
    {
        return Encoder.Encode(Compute(data));
    }

    public bool Verify(byte[] data, byte[] expected)
    {
        return CryptographicOperations.FixedTimeEquals(Compute(data), expected);
    }

    public bool Verify(string data, byte[] expected)
    {
        return CryptographicOperations.FixedTimeEquals(Compute(data), expected);
    }

    public bool VerifyEncoded(byte[] data, string expected)
    {
        return ComputeEncoded(data) == expected;
    }

    public bool VerifyEncoded(string data, string expected)
    {
        return ComputeEncoded(data) == expected;
    }

    public JsonObject ToJsonObject() => new()
    {
        [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = "MyCompany-Cryptography-XorHasher",
        [nameof(Encoder)] = Encoder.ToJsonObject()
    };

    public string Serialize(bool indented = false) =>
        ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));

    public static IHasher Deserialize(string json)
    {
        JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(XorHasher), json);

        if (jsonNode["TypeIdentifier"]?.GetValue<string>() != "MyCompany-Cryptography-XorHasher")
        {
            throw new DeserializeException(typeof(XorHasher), json);
        }

        return new XorHasher
        {
            Encoder = IEncoder.DeserializeDynamic(jsonNode[nameof(Encoder)]!.ToJsonString())
        };
    }
}
```

### Custom IEncryptor

This example uses XOR with a configurable key. It is intentionally simple for demonstration only and is not secure encryption.

```csharp
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Cryptography.Abstractions;
using InsaneIO.Insane.Cryptography.Attributes;
using InsaneIO.Insane.Exceptions;
using InsaneIO.Insane.Serialization;
using System.Text;
using System.Text.Json.Nodes;

[TypeIdentifier("MyCompany-Cryptography-XorEncryptor")]
public sealed class XorEncryptor : IEncryptor
{
    public byte[] KeyBytes { get; init; } = Encoding.UTF8.GetBytes("demo-key");
    public string KeyString { get => Encoding.UTF8.GetString(KeyBytes); init => KeyBytes = Encoding.UTF8.GetBytes(value); }
    public IEncoder Encoder { get; init; } = Base64Encoder.DefaultInstance;

    public byte[] Encrypt(byte[] data)
    {
        byte[] output = new byte[data.Length];

        for (int index = 0; index < data.Length; index++)
        {
            output[index] = (byte)(data[index] ^ KeyBytes[index % KeyBytes.Length]);
        }

        return output;
    }

    public byte[] Encrypt(string data)
    {
        return Encrypt(Encoding.UTF8.GetBytes(data));
    }

    public string EncryptEncoded(byte[] data)
    {
        return Encoder.Encode(Encrypt(data));
    }

    public string EncryptEncoded(string data)
    {
        return Encoder.Encode(Encrypt(data));
    }

    public byte[] Decrypt(byte[] data)
    {
        return Encrypt(data);
    }

    public byte[] DecryptEncoded(string data)
    {
        return Decrypt(Encoder.Decode(data));
    }

    public JsonObject ToJsonObject() => new()
    {
        [TypeIdentifierResolver.TypeIdentifierJsonPropertyName] = "MyCompany-Cryptography-XorEncryptor",
        [nameof(KeyBytes)] = Encoder.Encode(KeyBytes),
        [nameof(Encoder)] = Encoder.ToJsonObject()
    };

    public string Serialize(bool indented = false) =>
        ToJsonObject().ToJsonString(IJsonSerializable.GetIndentOptions(indented));

    public static IEncryptor Deserialize(string json)
    {
        JsonNode jsonNode = JsonNode.Parse(json) ?? throw new DeserializeException(typeof(XorEncryptor), json);

        if (jsonNode["TypeIdentifier"]?.GetValue<string>() != "MyCompany-Cryptography-XorEncryptor")
        {
            throw new DeserializeException(typeof(XorEncryptor), json);
        }

        IEncoder encoder = IEncoder.DeserializeDynamic(jsonNode[nameof(Encoder)]!.ToJsonString());

        return new XorEncryptor
        {
            Encoder = encoder,
            KeyBytes = encoder.Decode(jsonNode[nameof(KeyBytes)]!.GetValue<string>())
        };
    }
}
```

Once the assembly containing your custom implementation is loaded, `IEncoder.DeserializeDynamic`, `IHasher.DeserializeDynamic`, and `IEncryptor.DeserializeDynamic` can resolve it through `TypeIdentifier`.

## EncodingExtensions

Class: `InsaneIO.Insane.Extensions.EncodingExtensions`.

These extensions convert text to bytes and bytes to text using `System.Text.Encoding`.

## HexEncodingExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.HexEncodingExtensions`.

### DecodeFromHex

Behavior:

- Throws `ArgumentNullException` if the input is `null`
- Throws `ArgumentException` if the length is odd
- Throws if the input contains invalid hexadecimal characters

## Base64EncodingExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.Base64EncodingExtensions`.

Supports regular Base64, URL-safe Base64, filename-safe Base64, URL-encoded Base64, and line-broken Base64 values.

## Base32EncodingExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.Base32EncodingExtensions`.

Behavior:

- Accepts padded and unpadded Base32 values
- Validates padding position and legal padding lengths
- Rejects impossible Base32 lengths
- Throws `ArgumentException` for invalid input

## Encoders

All encoders implement `IEncoder` and `IEncoderJsonSerializable`.

Serialized encoder payloads include:

- `TypeIdentifier`
- encoder-specific settings

### HexEncoder

Properties:

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `ToUpper` | `bool` | `false` | Controls whether `Encode` uses uppercase hex digits |
| `DefaultInstance` | `HexEncoder` | Static instance | Reusable default instance |

### Base32Encoder

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `ToLower` | `bool` | `false` | Produces lowercase output during encoding |
| `RemovePadding` | `bool` | `false` | Omits `=` padding when `true` |
| `DefaultInstance` | `Base32Encoder` | Static instance | Reusable default instance |

### Base64Encoder

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `LineBreaksLength` | `uint` | `0` | Line length for regular Base64 output |
| `RemovePadding` | `bool` | `false` | Removes `=` when `EncodingType` is `Base64` |
| `EncodingType` | `Base64Encoding` | `Base64` | Selected Base64 flavor |
| `DefaultInstance` | `Base64Encoder` | Static instance | Reusable default instance |

## AesExtensions and AesCbcEncryptor

- String keys are converted to UTF-8 bytes
- Keys shorter than 8 bytes are rejected
- Keys are normalized by hashing them with SHA-512 and taking the first 32 bytes
- The encrypted result format is `ciphertext || iv`
- `DecryptAesCbc` validates that ciphertext length is at least 16 bytes so the IV exists

`AesCbcEncryptor` serializes:

- `TypeIdentifier`
- `Key`
- `Padding`
- `Encoder`

## RsaExtensions, RsaKeyPair, and RsaEncryptor

`RsaExtensions` provides:

- RSA key pair generation
- Public/private key validation
- Key encoding detection
- RSA encryption and decryption
- Encoded RSA encryption and decryption
- Async `IJSRuntime` helpers for Blazor WebAssembly

`RsaKeyPair.Deserialize(string json)` validates:

- the root `TypeIdentifier`
- that at least one key is present

`RsaEncryptor` serializes:

- `TypeIdentifier`
- `KeyPair`
- `Padding`
- `Encoder`

## HashExtensions, HmacExtensions, ScryptExtensions, Argon2Extensions

These classes live in `InsaneIO.Insane.Cryptography.Extensions`.

Security notes:

- `VerifyHash` and all `VerifyHashFromEncoded` overloads use constant-time comparison
- `VerifyHmac` and all `VerifyHmacFromEncoded` overloads use constant-time comparison
- `VerifyScrypt` and `VerifyScryptFromEncoded` use constant-time comparison
- `VerifyArgon2` and `VerifyArgon2FromEncoded` use constant-time comparison

## Hashers

### ShaHasher

Wraps plain hash computation with configurable `HashAlgorithm` and `Encoder`.

### HmacHasher

Wraps HMAC computation with configurable `HashAlgorithm`, `Encoder`, and secret key.

Serialized JSON contains the encoded HMAC key.

### ScryptHasher

Wraps Scrypt derivation with configurable salt and cost parameters.

### Argon2Hasher

Wraps Argon2 derivation with configurable salt, iterations, memory, parallelism, output size, variant, and encoder.

## TotpManager

Represents a serializable TOTP configuration.

It is the main TOTP entry point and now covers the convenience scenarios that older helper wrappers used to provide.

Useful members:

- `FromSecret(byte[] secret, string label, string issuer, ...)`
- `FromBase32Secret(string base32EncodedSecret, string label, string issuer, ...)`
- `FromEncodedSecret(string encodedSecret, IEncoder secretDecoder, string label, string issuer, ...)`
- `ToOtpUri()` and `GenerateTotpUri()`
- `ComputeCode()` and `ComputeCode(DateTimeOffset now)`
- `ComputeTotpCode()` and `ComputeTotpCode(DateTimeOffset now)`
- `VerifyCode(...)` and `VerifyTotpCode(...)`
- `ComputeRemainingSeconds()` / `ComputeTotpRemainingSeconds()`

`Deserialize(string json)` validates:

- the root `TypeIdentifier`
- that enum values are defined

## Dynamic Deserialization

The interfaces provide dynamic deserialization helpers:

```csharp
IEncoder encoder = IEncoder.DeserializeDynamic(jsonEncoder);
IHasher hasher = IHasher.DeserializeDynamic(jsonHasher);
IEncryptor encryptor = IEncryptor.DeserializeDynamic(jsonEncryptor);
```

Behavior:

- `DeserializeDynamic` resolves the concrete type through `TypeIdentifier`
- the type resolution uses an internal static cache so loaded assemblies are not rescanned on every deserialization call
- concrete `Deserialize(string json)` methods validate that the payload type matches the expected type
- if the payload belongs to another type or required fields are missing, they throw `DeserializeException`

## Sensitive Fields

Treat JSON containing these fields as sensitive:

- `AesCbcEncryptor.Key`
- `HmacHasher.Key`
- `RsaKeyPair.PrivateKey`
- `RsaEncryptor.KeyPair.PrivateKey`

## Notes

- AES-CBC encrypts but does not authenticate. Use a MAC or an authenticated encryption mode if one is added later.
- The AES output format in this library is `ciphertext || iv`, not `iv || ciphertext`.
- String keys are converted to UTF-8. If you need exact binary values, use `KeyBytes`, `SaltBytes`, or `byte[]` overloads.
- MD5 and SHA-1 are present for compatibility, but should not be used for new security-sensitive work.
- For modern RSA use cases, prefer OAEP padding. `Pkcs1` is mainly for compatibility.
- `IJSRuntime` methods use `eval` to register temporary functions. Review CSP restrictions if the code runs in browsers with strict policies.
