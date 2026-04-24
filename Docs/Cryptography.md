# Cryptography

This document describes the `InsaneIO.Insane.Cryptography` namespace, the shared contracts in `InsaneIO.Insane.Cryptography.Abstractions`, the cryptography extension methods in `InsaneIO.Insane.Cryptography.Extensions`, and the general-purpose helpers in `InsaneIO.Insane.Extensions`.

The library provides:

- Encoders for converting bytes to text: Base64, Base32, and Hex.
- General encoding extensions for converting strings and bytes with `System.Text.Encoding`.
- Plain hashes: MD5, SHA-1, SHA-256, SHA-384, and SHA-512.
- HMAC with the same algorithms.
- Key derivation with Scrypt and Argon2.
- Symmetric encryption with AES-CBC.
- Asymmetric encryption with RSA.
- Configurable and serializable classes: encryptors, hashers, and encoders.
- Blazor WebAssembly interop methods through `IJSRuntime`.

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

The serialization and runtime contracts now live in `InsaneIO.Insane.Cryptography.Abstractions`.

### IBaseSerializable

Defines the minimum serializable identity of a type.

| Member | Type | Description |
| --- | --- | --- |
| `SelfType` | `static abstract Type` | Concrete type implementing the interface. |
| `AssemblyName` | `string` | Fully qualified type name with assembly. Used to reconstruct concrete types during deserialization. |
| `GetName(Type implementationType)` | `static string` | Returns the name in `FullName, AssemblyName` format. |

For encoders, hashers, and encryptors, serialized payloads now also include a stable `CryptographyType` identifier so dynamic deserialization does not depend only on the CLR type name.

Example `AssemblyName` value:

```text
InsaneIO.Insane.Cryptography.Base64Encoder, InsaneIO.Insane
```

### IJsonSerializable

Extends `IBaseSerializable` for JSON-convertible objects.

| Member | Type | Description |
| --- | --- | --- |
| `ToJsonObject()` | `JsonObject` | Builds the JSON representation of the object. |
| `Serialize(bool indented = false)` | `string` | Returns the JSON as text. |
| `GetIndentOptions(bool indented)` | `static JsonSerializerOptions` | Returns serializer options with or without indentation. |

`Serialize` parameter:

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| `indented` | `bool` | `false` | When `true`, outputs formatted JSON with indentation and line breaks. |

### IEncoder

Common contract for byte-to-text encoders.

| Method | Return | Description |
| --- | --- | --- |
| `Encode(byte[] data)` | `string` | Encodes bytes to text. |
| `Encode(string data)` | `string` | Converts text to UTF-8 and encodes it. |
| `Decode(string data)` | `byte[]` | Decodes the text back to the original byte array. |
| `DeserializeDynamic(string json)` | `IEncoder` | Deserializes by reading `CryptographyType` first and falling back to `AssemblyName` when needed before dispatching to the correct concrete `Deserialize(string)` method. |

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
| `DeserializeDynamic(string json)` | `IEncryptor` | Deserializes by reading `CryptographyType` first and falling back to `AssemblyName` when needed before dispatching to the correct concrete `Deserialize(string)` method. |

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
| `DeserializeDynamic(string json)` | `IHasher` | Deserializes by reading `CryptographyType` first and falling back to `AssemblyName` when needed before dispatching to the correct concrete `Deserialize(string)` method. |

### IHasherJsonSerializable

Extends `IJsonSerializable` and requires:

```csharp
public static abstract IHasher Deserialize(string json);
```

### ISecureJsonSerializable

Planned contract for key-protected serialization. It currently defines the shape, but there are no implementations in the reviewed classes.

| Method | Return | Description |
| --- | --- | --- |
| `Serialize(byte[] serializeKey, bool indented = false)` | `string` | Serializes using a binary key. |
| `Serialize(string serializeKey, bool indented = false)` | `string` | Serializes using a text key. |
| `ToJsonObject(byte[] serializeKey)` | `JsonObject` | Creates protected JSON using a binary key. |
| `ToJsonObject(string serializeKey)` | `JsonObject` | Creates protected JSON using a text key. |

## EncodingExtensions

Class: `InsaneIO.Insane.Extensions.EncodingExtensions`.

These extensions convert text to bytes and bytes to text using `System.Text.Encoding`.

### ToByteArrayUtf8

```csharp
public static byte[] ToByteArrayUtf8(this string data)
```

Converts a string to UTF-8 bytes.

Example:

```csharp
byte[] bytes = "Hello".ToByteArrayUtf8();
```

### ToStringUtf8

```csharp
public static string ToStringUtf8(this byte[] utf8Bytes)
```

Converts UTF-8 bytes back to text.

Example:

```csharp
string text = bytes.ToStringUtf8();
```

### ToByteArray

```csharp
public static byte[] ToByteArray(this string data, Encoding encoding)
```

Converts a string to bytes using the provided encoding.

### ToString

```csharp
public static string ToString(this byte[] encodedBytes, Encoding encoding)
```

Converts bytes to text using the provided encoding.

## HexEncodingExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.HexEncodingExtensions`.

### EncodeToHex

```csharp
public static string EncodeToHex(this byte[] data, bool toUpper = false)
public static string EncodeToHex(this string data, bool toUpper = false)
```

Encodes bytes or UTF-8 text into hexadecimal.

Examples:

```csharp
string lower = "Hi".EncodeToHex();
string upper = new byte[] { 0x0a, 0xff }.EncodeToHex(true);
```

### DecodeFromHex

```csharp
public static byte[] DecodeFromHex(this string data)
```

Decodes hexadecimal text back to bytes.

Behavior:

- Throws `ArgumentNullException` if the input is `null`.
- Throws `ArgumentException` if the length is odd.
- Throws if the input contains invalid hexadecimal characters.

Example:

```csharp
byte[] bytes = "4869".DecodeFromHex();
string text = bytes.ToStringUtf8(); // Hi
```

## Base64EncodingExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.Base64EncodingExtensions`.

Constants:

| Constant | Value | Use |
| --- | --- | --- |
| `NoLineBreaks` | `0` | No line breaks. |
| `MimeLineBreaksLength` | `76` | Common MIME line length. |
| `PemLineBreaksLength` | `64` | Common PEM line length. |

### InsertLineBreaks

```csharp
public static string InsertLineBreaks(this string data, uint lineBreaksLength = MimeLineBreaksLength)
```

Inserts line breaks using `Environment.NewLine`.

### EncodeToBase64

```csharp
public static string EncodeToBase64(this byte[] data, uint lineBreaksLength = NoLineBreaks, bool removePadding = false)
public static string EncodeToBase64(this string data, uint lineBreaksLength = NoLineBreaks, bool removePadding = false)
```

Encodes bytes or UTF-8 text as Base64.

Examples:

```csharp
string base64 = "Hello".EncodeToBase64();
string pemBody = bytes.EncodeToBase64(Base64EncodingExtensions.PemLineBreaksLength);
string compact = "Hello".EncodeToBase64(removePadding: true);
```

### EncodeToUrlSafeBase64

```csharp
public static string EncodeToUrlSafeBase64(this byte[] data)
```

Converts Base64 to a URL-safe variant:

- `+` becomes `-`
- `/` becomes `_`
- `=` is removed

### EncodeToFilenameSafeBase64

Alias of `EncodeToUrlSafeBase64`.

### EncodeToUrlEncodedBase64

Converts Base64 to a URL-encoded variant by replacing:

- `+` with `%2B`
- `/` with `%2F`
- `=` with `%3D`

### DecodeFromBase64

```csharp
public static byte[] DecodeFromBase64(this string data)
```

Accepts regular Base64, no-padding Base64, URL-safe Base64, filename-safe Base64, URL-encoded Base64, and values containing line breaks.

Behavior:

- Trims the input.
- Converts `%2B`, `%2F`, `%3D` back to `+`, `/`, `=`.
- Converts `-`, `_` back to `+`, `/`.
- Removes `\r` and `\n`.
- Restores `=` padding up to a multiple of 4.
- Calls `Convert.FromBase64String`.

## Base32EncodingExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.Base32EncodingExtensions`.

### EncodeToBase32

```csharp
public static string EncodeToBase32(this byte[] data, bool removePadding = false, bool toLower = false)
public static string EncodeToBase32(this string data, bool removePadding = false, bool toLower = false)
```

Encodes bytes or UTF-8 text as Base32.

### DecodeFromBase32

```csharp
public static byte[] DecodeFromBase32(this string data)
```

Behavior:

- Trims the input.
- Accepts padded and unpadded Base32 values.
- Validates that padding appears only at the end.
- Validates legal Base32 padding lengths.
- Rejects impossible Base32 lengths.
- Accepts uppercase and lowercase letters and digits `2-7`.
- Throws `ArgumentException` for invalid input.

## Encoders

All encoders implement `IEncoder` and `IEncoderJsonSerializable`.

Serialized encoder payloads include both:

- `CryptographyType`: stable identifier declared through `CryptographyTypeAttribute`
- `AssemblyName`: CLR type fallback for backward compatibility

### HexEncoder

Class: `InsaneIO.Insane.Cryptography.HexEncoder`.

Properties:

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `ToUpper` | `bool` | `false` | Controls whether `Encode` uses uppercase hex digits. |
| `AssemblyName` | `string` | Computed | Type identifier used during serialization. |
| `DefaultInstance` | `HexEncoder` | Static instance | Reusable default instance. |

Methods:

| Method | Return | Description |
| --- | --- | --- |
| `Encode(byte[] data)` | `string` | Calls `EncodeToHex(ToUpper)`. |
| `Encode(string data)` | `string` | Converts to UTF-8 and encodes. |
| `Decode(string data)` | `byte[]` | Calls `DecodeFromHex()`. |
| `Serialize(bool indented = false)` | `string` | Serializes the configuration. |
| `Deserialize(string json)` | `IEncoder` | Restores a `HexEncoder` from JSON. Validates the root type. |

### Base32Encoder

Class: `InsaneIO.Insane.Cryptography.Base32Encoder`.

Properties:

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `ToLower` | `bool` | `false` | Produces lowercase output during encoding. |
| `RemovePadding` | `bool` | `false` | Omits `=` padding when `true`. |
| `AssemblyName` | `string` | Computed | Type identifier used during serialization. |
| `DefaultInstance` | `Base32Encoder` | Static instance | Reusable default instance. |

`Deserialize(string json)` validates the root type before restoring the object.

### Base64Encoder

Class: `InsaneIO.Insane.Cryptography.Base64Encoder`.

Properties:

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `LineBreaksLength` | `uint` | `0` | Line length for regular Base64 output. |
| `RemovePadding` | `bool` | `false` | Removes `=` when `EncodingType` is `Base64`. |
| `EncodingType` | `Base64Encoding` | `Base64` | Selected Base64 flavor. |
| `AssemblyName` | `string` | Computed | Type identifier used during serialization. |
| `DefaultInstance` | `Base64Encoder` | Static instance | Reusable default instance. |

`Deserialize(string json)` validates the root type before restoring the object.

## AesExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.AesExtensions`.

Constants:

| Constant | Value | Description |
| --- | --- | --- |
| `MaxIvLength` | `16` | AES IV size stored in encrypted payloads. |
| `MaxKeyLength` | `32` | Final normalized AES key size. |

Behavior:

- String keys are converted to UTF-8 bytes.
- Keys shorter than 8 bytes are rejected.
- Keys are normalized by hashing them with SHA-512 and taking the first 32 bytes.
- The encrypted result format is `ciphertext || iv`.
- `DecryptAesCbc` now validates that ciphertext length is at least 16 bytes so the IV exists, and fails early with a clear exception for truncated payloads.

Common methods:

```csharp
EncryptAesCbc(...)
EncryptAesCbcEncoded(...)
DecryptAesCbc(...)
DecryptAesCbcFromEncoded(...)
```

## AesCbcEncryptor

Class: `InsaneIO.Insane.Cryptography.AesCbcEncryptor`.

Implements `IEncryptor`.

Properties:

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `KeyString` | `string` | Random key | Setter converts to UTF-8. Getter returns the key encoded with `Encoder`. |
| `KeyBytes` | `byte[]` | Random | Binary key bytes. |
| `Encoder` | `IEncoder` | `Base64Encoder.DefaultInstance` | Encoder used by encoded methods and serialization. |
| `Padding` | `AesCbcPadding` | `Pkcs7` | AES-CBC padding strategy. |
| `AssemblyName` | `string` | Computed | Type identifier. |

`Deserialize(string json)` validates the root type before restoring the encryptor.

## RsaExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.RsaExtensions`.

Provides:

- RSA key pair generation.
- Public/private key validation.
- Key encoding detection.
- RSA encryption and decryption.
- Encoded RSA encryption and decryption.
- Async `IJSRuntime` helpers for Blazor WebAssembly.

Supported key encodings:

- BER public/private
- PEM public/private
- XML public/private

Common methods:

```csharp
CreateRsaKeyPair(...)
ValidateRsaPublicKey(...)
ValidateRsaPrivateKey(...)
GetRsaKeyEncoding(...)
EncryptRsa(...)
EncryptRsaEncoded(...)
DecryptRsa(...)
DecryptRsaFromEncoded(...)
```

## RsaKeyPair

Class: `InsaneIO.Insane.Cryptography.RsaKeyPair`.

Properties:

| Property | Type | Description |
| --- | --- | --- |
| `PublicKey` | `string?` | Public key in the selected encoding. |
| `PrivateKey` | `string?` | Private key in the selected encoding. |
| `AssemblyName` | `string` | Type identifier used during serialization. |

`Deserialize(string json)` now validates:

- the root `AssemblyName`
- that at least one key is present

## RsaEncryptor

Class: `InsaneIO.Insane.Cryptography.RsaEncryptor`.

Implements `IEncryptor`.

Properties:

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `KeyPair` | `RsaKeyPair` | Required | Public key is used for encryption, private key for decryption. |
| `Padding` | `RsaPadding` | `OaepSha256` | RSA padding mode. |
| `Encoder` | `IEncoder` | `Base64Encoder.DefaultInstance` | Encoder used by encoded methods and serialization. |
| `AssemblyName` | `string` | Computed | Type identifier. |

`Deserialize(string json)` validates the root type and reconstructs nested objects through their serialization contracts.

## HashExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.HashExtensions`.

Provides:

- Plain hashing
- Encoded variants
- Verification helpers

Security note:

- `VerifyHash` and all `FromEncoded` hash verification helpers use constant-time comparison to reduce timing side channels when comparing secrets and derived values.

Example:

```csharp
string hash = "Hello".ComputeHashEncoded(HexEncoder.DefaultInstance, HashAlgorithm.Sha256);
bool ok = "Hello".VerifyHashFromEncoded(hash, HexEncoder.DefaultInstance, HashAlgorithm.Sha256);
```

## HmacExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.HmacExtensions`.

Provides:

- HMAC with `Md5`, `Sha1`, `Sha256`, `Sha384`, and `Sha512`
- String and byte-array overloads for both payload and key
- Encoded variants
- Verification helpers

Security note:

- `VerifyHmac` and all `VerifyHmacFromEncoded` overloads use constant-time comparison.

Example:

```csharp
string mac = "Hello".ComputeHmacEncoded("secret", Base64Encoder.DefaultInstance, HashAlgorithm.Sha256);
bool ok = "Hello".VerifyHmacFromEncoded("secret", mac, Base64Encoder.DefaultInstance, HashAlgorithm.Sha256);
```

## ScryptExtensions

Class: `InsaneIO.Insane.Cryptography.Extensions.ScryptExtensions`.

Provides:

- Raw Scrypt derivation
- Encoded variants
- Verification helpers
- String and byte-array overloads for data and salt

Security note:

- `VerifyScrypt` and `VerifyScryptFromEncoded` use constant-time comparison.

Example:

```csharp
string derived = "password".ComputeScryptEncoded(
    "salt",
    HexEncoder.DefaultInstance,
    iterations: 16,
    blockSize: 1,
    parallelism: 1,
    derivedKeyLength: 16);

bool ok = "password".VerifyScryptFromEncoded(
    "salt",
    derived,
    HexEncoder.DefaultInstance,
    iterations: 16,
    blockSize: 1,
    parallelism: 1,
    derivedKeyLength: 16);
```

## Argon2Extensions

Class: `InsaneIO.Insane.Cryptography.Extensions.Argon2Extensions`.

Provides:

- Raw Argon2 derivation
- Support for `Argon2d`, `Argon2i`, and `Argon2id`
- Encoded variants
- Verification helpers
- String and byte-array overloads for data and salt

Security note:

- `VerifyArgon2` and `VerifyArgon2FromEncoded` use constant-time comparison.

Example:

```csharp
string derived = "password".ComputeArgon2Encoded(
    "salt",
    Base64Encoder.DefaultInstance,
    iterations: 1,
    memorySizeKiB: 1024,
    parallelism: 1,
    variant: Argon2Variant.Argon2id,
    derivedKeyLength: 16);

bool ok = "password".VerifyArgon2FromEncoded(
    "salt",
    derived,
    Base64Encoder.DefaultInstance,
    iterations: 1,
    memorySizeKiB: 1024,
    parallelism: 1,
    variant: Argon2Variant.Argon2id,
    derivedKeyLength: 16);
```

## Hashers

### ShaHasher

Wraps plain hash computation with configurable `HashAlgorithm` and `Encoder`.

`Deserialize(string json)` validates the root type and restores the configured encoder.

### HmacHasher

Wraps HMAC computation with configurable `HashAlgorithm`, `Encoder`, and secret key.

Security note:

- Serialized JSON contains the encoded HMAC key.

### ScryptHasher

Wraps Scrypt derivation with configurable salt and cost parameters.

### Argon2Hasher

Wraps Argon2 derivation with configurable salt, iterations, memory, parallelism, output size, variant, and encoder.

## TotpManager

Class: `InsaneIO.Insane.Security.TotpManager`.

Represents a serializable TOTP configuration.

Properties:

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `Secret` | `byte[]` | Required | Raw TOTP secret. |
| `Label` | `string` | Required | Account label used in OTP URIs. |
| `Issuer` | `string` | Required | Issuer used in OTP URIs. |
| `CodeLength` | `TwoFactorCodeLength` | `SixDigits` | Number of digits in generated codes. |
| `HashAlgorithm` | `HashAlgorithm` | `Sha1` | TOTP hash algorithm. |
| `TimePeriodInSeconds` | `uint` | Default TOTP period | TOTP step length in seconds. |

`Deserialize(string json)` now validates:

- the root `AssemblyName`
- that enum values are defined

## Dynamic Deserialization

The JSON produced by encoders, hashers, and encryptors includes both `CryptographyType` and `AssemblyName`. The interfaces provide dynamic deserialization helpers:

```csharp
IEncoder encoder = IEncoder.DeserializeDynamic(jsonEncoder);
IHasher hasher = IHasher.DeserializeDynamic(jsonHasher);
IEncryptor encryptor = IEncryptor.DeserializeDynamic(jsonEncryptor);
```

Behavior:

- `DeserializeDynamic` first tries to resolve the concrete type through `CryptographyType`.
- If the identifier is missing or cannot be resolved, it falls back to `AssemblyName`.
- The type resolution uses an internal static cache so loaded assemblies are not rescanned on every deserialization call.
- The concrete `Deserialize(string json)` methods validate that the payload type matches the expected type through `CryptographyType` first and `AssemblyName` as fallback.
- If the payload belongs to another type or required fields are missing, they throw `DeserializeException`.

## Sensitive Fields

Treat JSON containing these fields as sensitive:

- `AesCbcEncryptor.Key`
- `HmacHasher.Key`
- `RsaKeyPair.PrivateKey`
- `RsaEncryptor.KeyPair.PrivateKey`

Scrypt and Argon2 salts are not secrets, but they must be preserved to reproduce the same derived result.

## Integrated Examples

### AES + HMAC

AES-CBC encrypts but does not authenticate. A safer pattern is encrypt-then-MAC.

```csharp
var aes = new AesCbcEncryptor
{
    KeyString = "12345678",
    Encoder = Base64Encoder.DefaultInstance
};

var hmac = new HmacHasher
{
    KeyString = "mac-secret",
    HashAlgorithm = HashAlgorithm.Sha256,
    Encoder = Base64Encoder.DefaultInstance
};

string encrypted = aes.EncryptEncoded("payload");
string mac = hmac.ComputeEncoded(encrypted);

bool valid = hmac.VerifyEncoded(encrypted, mac);
string decrypted = valid ? aes.DecryptEncoded(encrypted).ToStringUtf8() : throw new CryptographicException();
```

### RSA + AES for Large Payloads

```csharp
RsaKeyPair rsaKeys = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);
byte[] aesKey = RandomExtensions.NextBytes(32);

string encryptedPayload = "large payload".EncryptAesCbcEncoded(
    aesKey,
    Base64Encoder.DefaultInstance);

string encryptedKey = aesKey.EncryptRsaEncoded(
    rsaKeys.PublicKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256);

byte[] restoredKey = encryptedKey.DecryptRsaFromEncoded(
    rsaKeys.PrivateKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256);

string payload = encryptedPayload
    .DecryptAesCbcFromEncoded(restoredKey, Base64Encoder.DefaultInstance)
    .ToStringUtf8();
```

### Password Hashing with Argon2Hasher

```csharp
var passwordHasher = new Argon2Hasher
{
    SaltBytes = RandomExtensions.NextBytes(Constants.Argon2SaltSize),
    Argon2Variant = Argon2Variant.Argon2id,
    Encoder = Base64Encoder.DefaultInstance
};

string passwordHash = passwordHasher.ComputeEncoded("user-password");
string hasherConfig = passwordHasher.Serialize();

IHasher restored = Argon2Hasher.Deserialize(hasherConfig);
bool ok = restored.VerifyEncoded("user-password", passwordHash);
```

## Notes

- AES-CBC encrypts but does not authenticate. Use a MAC or an authenticated encryption mode if one is added later.
- The AES output format in this library is `ciphertext || iv`, not `iv || ciphertext`.
- String keys are converted to UTF-8. If you need exact binary values, use `KeyBytes`, `SaltBytes`, or `byte[]` overloads.
- MD5 and SHA-1 are present for compatibility, but should not be used for new security-sensitive work.
- For modern RSA use cases, prefer OAEP padding. `Pkcs1` is mainly for compatibility.
- `IJSRuntime` methods use `eval` to register temporary functions. Review CSP restrictions if the code runs in browsers with strict policies.

