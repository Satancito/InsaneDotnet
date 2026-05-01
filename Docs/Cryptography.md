# Cryptography

This document is the single-file reference for the public cryptography surface of `InsaneIO.Insane`.

It covers:

- public cryptography namespaces
- concrete cryptography classes
- interfaces and serialization contracts
- enums
- extension/helper APIs
- dynamic deserialization
- `IJSRuntime` cryptography helpers where they are publicly exposed

For readers who prefer the namespace tree, the structured index is still available at [Docs/namespaces/Namespaces.md](namespaces/Namespaces.md).

## Covered namespaces

- `InsaneIO.Insane.Cryptography`
- `InsaneIO.Insane.Cryptography.Abstractions`
- `InsaneIO.Insane.Cryptography.Enums`
- `InsaneIO.Insane.Cryptography.Extensions`
- `InsaneIO.Insane.Serialization`
- `InsaneIO.Insane.Attributes`
- selected shared helpers from `InsaneIO.Insane.Extensions` and `InsaneIO.Insane`

## How the cryptography API is organized

The package gives you two main ways to work:

1. Direct helper/extension calls
2. Configurable serializable objects

### Direct helpers

Use extension methods when you want one-off work without object setup.

```csharp
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Cryptography.Enums;
using InsaneIO.Insane.Cryptography.Extensions;

string sha256 = "payload".ComputeHashEncoded(HexEncoder.DefaultInstance, HashAlgorithm.Sha256);
string cipher = "payload".EncryptAesCbcEncoded("password", Base64Encoder.DefaultInstance, AesCbcPadding.Pkcs7);
```

### Configurable objects

Use concrete classes when you want:

- reusable configuration
- serialization/deserialization
- dependency injection or stateful composition
- polymorphic storage through interfaces

```csharp
using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Cryptography.Enums;

var hasher = new ShaHasher
{
    HashAlgorithm = HashAlgorithm.Sha256,
    Encoder = HexEncoder.DefaultInstance
};

string digest = hasher.ComputeEncoded("payload");
string json = hasher.Serialize(indented: true);
```

### String overloads

Across the cryptography API, overloads that accept `string` convert the input to UTF-8 bytes before processing.

### Encoded overloads

Methods ending in `Encoded` return text by passing raw bytes through an `IEncoder`.

Methods containing `FromEncoded` expect already encoded text, decode it through the supplied `IEncoder`, and continue with the cryptographic operation.

## TypeIdentifier-based serialization and dynamic deserialization

The current design no longer uses CLR type names or assembly names as public serialized identity.

Instead, concrete serializable cryptography types are decorated with:

- `TypeIdentifierAttribute`

and serialized with a root JSON property:

- `TypeIdentifier`

### Public pieces involved

- `TypeIdentifierAttribute`
- `IJsonSerializable`
- `TypeIdentifierResolver`
- `IEncoder.DeserializeDynamic(string json)`
- `IHasher.DeserializeDynamic(string json)`
- `IEncryptor.DeserializeDynamic(string json)`

### Resolution rules

Dynamic deserialization:

1. parses the JSON
2. reads `TypeIdentifier`
3. resolves a unique decorated concrete type
4. validates that the resolved type implements the expected contract
5. invokes the concrete static `Deserialize(string json)` method

There is no fallback to:

- `AssemblyName`
- CLR type names
- class names

### Practical implication

If the JSON is missing `TypeIdentifier`, uses the wrong one, or points to a type that does not implement the requested interface, deserialization fails with `DeserializeException`.

## Public contracts

### IJsonSerializable

Namespace: `InsaneIO.Insane.Serialization`

Shared JSON contract:

- `JsonObject ToJsonObject()`
- `string Serialize(bool indented = false)`
- `static JsonSerializerOptions GetIndentOptions(bool indented)`

### IEncoder

Namespace: `InsaneIO.Insane.Cryptography.Abstractions`

Represents a byte-to-text codec.

Members:

- `Encode(byte[] data)`
- `Encode(string data)`
- `Decode(string data)`
- `static DeserializeDynamic(string json)`

### IHasher

Represents a configured digest, HMAC, or key-derivation component.

Members:

- `Compute(...)`
- `ComputeEncoded(...)`
- `Verify(...)`
- `VerifyEncoded(...)`
- `static DeserializeDynamic(string json)`

### IEncryptor

Represents a configured encryption component.

Members:

- `Encrypt(...)`
- `EncryptEncoded(...)`
- `Decrypt(...)`
- `DecryptEncoded(...)`
- `static DeserializeDynamic(string json)`

### IRsaKeyPairSerializable

Serialization contract for `RsaKeyPair`.

## Public enums

### AesCbcPadding

- `None`
- `Zeros`
- `Pkcs7`
- `AnsiX923`

Controls AES-CBC padding behavior.

### Argon2Variant

- `Argon2d`
- `Argon2i`
- `Argon2id`

Controls which Argon2 family is used.

### Base64Encoding

- `Base64`
- `UrlSafeBase64`
- `FileNameSafeBase64`
- `UrlEncodedBase64`

Controls `Base64Encoder` output mode.

### EncoderType

- `Hex`
- `Base32`
- `Base64`

Logical encoder families.

### HashAlgorithm

- `Md5`
- `Sha1`
- `Sha256`
- `Sha384`
- `Sha512`

Used by hashing, HMAC, and TOTP configuration. For TOTP specifically, `Md5` and `Sha384` are normalized to `Sha1`.

### RsaKeyEncoding

- `Unknown`
- `BerPublic`
- `BerPrivate`
- `PemPublic`
- `PemPrivate`
- `XmlPublic`
- `XmlPrivate`

Represents detected format for a single RSA key.

### RsaKeyPairEncoding

- `Ber`
- `Pem`
- `Xml`

Controls the output format of generated key pairs.

### RsaPadding

- `Pkcs1`
- `OaepSha1`
- `OaepSha256`
- `OaepSha384`
- `OaepSha512`

Controls RSA encryption padding.

## Encoders

## Base32Encoder

Namespace: `InsaneIO.Insane.Cryptography`

`Base32Encoder` is a configurable Base32 codec with control over:

- lowercase output
- optional padding removal

### Properties

- `ToLower`
- `RemovePadding`
- `DefaultInstance`

Defaults:

- `ToLower = false`
- `RemovePadding = false`

### Methods

- `Encode(byte[] data)`
- `Encode(string data)`
- `Decode(string data)`
- `ToJsonObject()`
- `Serialize(bool indented = false)`
- `static Deserialize(string json)`

### Serialization

JSON shape:

```json
{
  "TypeIdentifier": "Insane-Cryptography-Base32Encoder",
  "RemovePadding": false,
  "ToLower": false
}
```

`Deserialize(...)` requires:

- correct `TypeIdentifier`
- `RemovePadding`
- `ToLower`

### Typical usage

```csharp
var encoder = new Base32Encoder
{
    RemovePadding = true,
    ToLower = true
};

string text = encoder.Encode("insane");
byte[] bytes = encoder.Decode(text);
```

## Base64Encoder

`Base64Encoder` wraps the Base64 extension family in an object-oriented, serializable form.

### Properties

- `LineBreaksLength`
- `RemovePadding`
- `EncodingType`
- `DefaultInstance`

Defaults:

- `LineBreaksLength = 0`
- `RemovePadding = false`
- `EncodingType = Base64Encoding.Base64`

Exposed constants:

- `NoLineBreaks = 0`
- `MimeLineBreaksLength = 76`
- `PemLineBreaksLength = 64`

### Supported output modes

- regular Base64
- URL-safe Base64
- filename-safe Base64
- URL-encoded Base64

### Methods

- `Encode(byte[] data)`
- `Encode(string data)`
- `Decode(string data)`
- `ToJsonObject()`
- `Serialize(...)`
- `static Deserialize(string json)`

### Serialization

JSON shape:

```json
{
  "TypeIdentifier": "Insane-Cryptography-Base64Encoder",
  "LineBreaksLength": 0,
  "RemovePadding": false,
  "EncodingType": 0
}
```

`EncodingType` is serialized numerically.

`Deserialize(...)` rejects:

- missing `TypeIdentifier`
- wrong type identifier
- missing `LineBreaksLength`
- missing `RemovePadding`
- missing `EncodingType`
- undefined enum values

### Example

```csharp
var encoder = new Base64Encoder
{
    LineBreaksLength = Base64Encoder.MimeLineBreaksLength,
    RemovePadding = false,
    EncodingType = Base64Encoding.Base64
};

string encoded = encoder.Encode("payload");
byte[] decoded = encoder.Decode(encoded);
```

## HexEncoder

`HexEncoder` wraps lowercase or uppercase hexadecimal text encoding.

### Properties

- `ToUpper`
- `DefaultInstance`

Default:

- `ToUpper = false`

### Methods

- `Encode(byte[] data)`
- `Encode(string data)`
- `Decode(string data)`
- `Serialize(...)`
- `static Deserialize(string json)`

### Serialization

JSON shape:

```json
{
  "TypeIdentifier": "Insane-Cryptography-HexEncoder",
  "ToUpper": true
}
```

### Example

```csharp
var encoder = new HexEncoder { ToUpper = true };
string encoded = encoder.Encode("payload");
byte[] raw = encoder.Decode(encoded);
```

## Hashers

## ShaHasher

`ShaHasher` is the serializable object wrapper for plain hash computation.

### Properties

- `HashAlgorithm`
- `Encoder`

Defaults:

- `HashAlgorithm = Sha512`
- `Encoder = Base64Encoder.DefaultInstance`

### Methods

- `Compute(byte[] data)`
- `Compute(string data)`
- `ComputeEncoded(byte[] data)`
- `ComputeEncoded(string data)`
- `Verify(...)`
- `VerifyEncoded(...)`
- `Serialize(...)`
- `static Deserialize(string json)`

### Behavior

- uses the selected hash algorithm
- string inputs are UTF-8
- encoded output uses the configured encoder

### Serialization

JSON shape:

```json
{
  "TypeIdentifier": "Insane-Cryptography-ShaHasher",
  "HashAlgorithm": 2,
  "Encoder": { ... }
}
```

### Example

```csharp
var hasher = new ShaHasher
{
    HashAlgorithm = HashAlgorithm.Sha256,
    Encoder = HexEncoder.DefaultInstance
};

string digest = hasher.ComputeEncoded("payload");
bool ok = hasher.VerifyEncoded("payload", digest);
```

## HmacHasher

`HmacHasher` is the serializable object wrapper for keyed HMAC computation.

### Properties

- `HashAlgorithm`
- `Encoder`
- `KeyString`
- `KeyBytes`

Defaults:

- `HashAlgorithm = Sha512`
- `Encoder = Base64Encoder.DefaultInstance`
- internal random key generated automatically

### Key configuration

- use `KeyString` for convenient textual initialization
- use `KeyBytes` when exact binary material matters

### Methods

Same method family as `ShaHasher`:

- `Compute(...)`
- `ComputeEncoded(...)`
- `Verify(...)`
- `VerifyEncoded(...)`
- serialization methods

### Serialization

The key is serialized encoded through the configured encoder.

### Example

```csharp
var hasher = new HmacHasher
{
    KeyString = "shared-secret",
    HashAlgorithm = HashAlgorithm.Sha384,
    Encoder = Base64Encoder.DefaultInstance
};

string mac = hasher.ComputeEncoded("payload");
```

## ScryptHasher

`ScryptHasher` wraps SCrypt key derivation in a reusable object.

### Properties

- `SaltString`
- `SaltBytes`
- `Iterations`
- `BlockSize`
- `Parallelism`
- `DerivedKeyLength`
- `Encoder`

Defaults come from `Constants`:

- `ScryptIterations`
- `ScryptBlockSize`
- `ScryptParallelism`
- `ScryptDerivedKeyLength`
- random salt of `ScryptSaltSize`

### Methods

- `Compute(...)`
- `ComputeEncoded(...)`
- `Verify(...)`
- `VerifyEncoded(...)`
- serialization methods

### Typical usage

```csharp
var hasher = new ScryptHasher
{
    SaltString = "salt",
    Iterations = 16384,
    BlockSize = 8,
    Parallelism = 1,
    DerivedKeyLength = 64,
    Encoder = Base64Encoder.DefaultInstance
};

string derived = hasher.ComputeEncoded("password");
```

## Argon2Hasher

`Argon2Hasher` wraps Argon2 derivation with fully configurable work factors.

### Properties

- `SaltString`
- `SaltBytes`
- `Encoder`
- `Iterations`
- `MemorySizeKiB`
- `DegreeOfParallelism`
- `DerivedKeyLength`
- `Argon2Variant`

Defaults:

- random salt
- `Iterations = Constants.Argon2Iterations`
- `MemorySizeKiB = Constants.Argon2MemorySizeInKiB`
- `DegreeOfParallelism = Constants.Argon2DegreeOfParallelism`
- `DerivedKeyLength = Constants.Argon2DerivedKeyLength`
- `Argon2Variant = Argon2Variant.Argon2id`

### Methods

- `Compute(...)`
- `ComputeEncoded(...)`
- `Verify(...)`
- `VerifyEncoded(...)`
- serialization methods

### Example

```csharp
var hasher = new Argon2Hasher
{
    SaltString = "salt",
    Iterations = 2,
    MemorySizeKiB = 16384,
    DegreeOfParallelism = 4,
    DerivedKeyLength = 64,
    Argon2Variant = Argon2Variant.Argon2id,
    Encoder = Base64Encoder.DefaultInstance
};

string derived = hasher.ComputeEncoded("password");
```

## Encryptors

## AesCbcEncryptor

`AesCbcEncryptor` is the serializable object wrapper for AES-CBC operations.

### Properties

- `KeyString`
- `KeyBytes`
- `Encoder`
- `Padding`

Defaults:

- random internal key
- `Encoder = Base64Encoder.DefaultInstance`
- `Padding = AesCbcPadding.Pkcs7`

### Important behavior

- key input must be at least 8 bytes
- the key is normalized by hashing with SHA-512 and taking the first 32 bytes
- ciphertext format is `ciphertext || iv`

### Methods

- `Encrypt(byte[] data)`
- `Encrypt(string data)`
- `EncryptEncoded(byte[] data)`
- `EncryptEncoded(string data)`
- `Decrypt(byte[] data)`
- `DecryptEncoded(string data)`
- `Serialize(...)`
- `static Deserialize(string json)`

### Example

```csharp
var encryptor = new AesCbcEncryptor
{
    KeyString = "super-secret-password",
    Encoder = Base64Encoder.DefaultInstance,
    Padding = AesCbcPadding.Pkcs7
};

string cipher = encryptor.EncryptEncoded("payload");
string clear = encryptor.DecryptEncoded(cipher).ToStringUtf8();
```

## RsaKeyPair

`RsaKeyPair` is the serializable model for a public/private RSA pair.

### Properties

- `PublicKey`
- `PrivateKey`

### Serialization

JSON shape:

```json
{
  "TypeIdentifier": "Insane-Cryptography-RsaKeyPair",
  "PublicKey": "...",
  "PrivateKey": "..."
}
```

Current `Deserialize(...)` behavior requires both keys to be present and non-whitespace.

### Typical flow

```csharp
RsaKeyPair pair = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);
string json = pair.Serialize(indented: true);
RsaKeyPair restored = RsaKeyPair.Deserialize(json);
```

## RsaEncryptor

`RsaEncryptor` is the serializable object wrapper for RSA encryption/decryption.

### Properties

- `KeyPair` (required)
- `Padding`
- `Encoder`

Defaults:

- `Padding = RsaPadding.OaepSha256`
- `Encoder = Base64Encoder.DefaultInstance`

### Methods

- `Encrypt(...)`
- `EncryptEncoded(...)`
- `Decrypt(...)`
- `DecryptEncoded(...)`
- serialization methods

### Example

```csharp
RsaKeyPair pair = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);

var encryptor = new RsaEncryptor
{
    KeyPair = pair,
    Padding = RsaPadding.OaepSha256,
    Encoder = Base64Encoder.DefaultInstance
};

string cipher = encryptor.EncryptEncoded("payload");
string clear = encryptor.DecryptEncoded(cipher).ToStringUtf8();
```

## Encoding helpers

## Base32EncodingExtensions

### Method family

- `EncodeToBase32(this byte[] data, bool removePadding = false, bool toLower = false)`
- `EncodeToBase32(this string data, bool removePadding = false, bool toLower = false)`
- `DecodeFromBase32(this string data)`

### Behavior

- accepts uppercase and lowercase input
- trims outer whitespace
- accepts padded and unpadded values
- rejects impossible lengths and illegal padding patterns

### Example

```csharp
string padded = "hello".ToByteArrayUtf8().EncodeToBase32();
string compact = "hello".ToByteArrayUtf8().EncodeToBase32(removePadding: true, toLower: true);
byte[] back = compact.DecodeFromBase32();
```

## Base64EncodingExtensions

### Method family

- `InsertLineBreaks(...)`
- `EncodeToBase64(...)`
- `EncodeToUrlSafeBase64(...)`
- `EncodeToFilenameSafeBase64(...)`
- `EncodeToUrlEncodedBase64(...)`
- `DecodeFromBase64(...)`
- `EncodeBase64ToUrlSafeBase64(...)`
- `EncodeBase64ToFilenameSafeBase64(...)`
- `EncodeBase64ToUrlEncodedBase64(...)`

### Behavior

- regular string overloads use UTF-8
- decoder accepts standard, URL-safe, filename-safe, URL-encoded, and line-broken Base64
- `InsertLineBreaks(...)` uses `Environment.NewLine`

## HexEncodingExtensions

### Method family

- `EncodeToHex(...)`
- `DecodeFromHex(...)`

### Behavior

- string inputs use UTF-8
- odd lengths are rejected
- invalid characters are rejected with `ArgumentException`

## Hash and HMAC helpers

## HashExtensions

### Method family

- `ComputeHash(...)`
- `ComputeHashEncoded(...)`
- `VerifyHash(...)`
- `VerifyHashFromEncoded(...)`

### Supported algorithms

- `Md5`
- `Sha1`
- `Sha256`
- `Sha384`
- `Sha512`

### Comparison behavior

Verification uses fixed-time comparison helpers.

## HmacExtensions

### Method family

- `ComputeHmac(...)`
- `ComputeHmacEncoded(...)`
- `VerifyHmac(...)`
- `VerifyHmacFromEncoded(...)`

### Overloads

Supports both byte and string forms for:

- data
- key

String values are converted using UTF-8.

## Key derivation helpers

## ScryptExtensions

### Method family

- `ComputeScrypt(...)`
- `ComputeScryptEncoded(...)`
- `VerifyScrypt(...)`
- `VerifyScryptFromEncoded(...)`

### Parameters

- `iterations`
- `blockSize`
- `parallelism`
- `derivedKeyLength`

Defaults come from `Constants`.

## Argon2Extensions

### Method family

- `ComputeArgon2(...)`
- `ComputeArgon2Encoded(...)`
- `VerifyArgon2(...)`
- `VerifyArgon2FromEncoded(...)`

### Parameters

- `iterations`
- `memorySizeKiB`
- `parallelism`
- `variant`
- `derivedKeyLength`

## Symmetric encryption helpers

## AesExtensions

### Public constants

- `MaxIvLength = 16`
- `MaxKeyLength = 32`

### Method family

- `EncryptAesCbc(...)`
- `DecryptAesCbc(...)`
- `EncryptAesCbcEncoded(...)`
- `DecryptAesCbcFromEncoded(...)`
- async `IJSRuntime` versions of the same operations

### Validation

- keys shorter than 8 bytes are rejected
- ciphertext shorter than the IV length is rejected

### IJSRuntime support

The public async overloads register temporary JavaScript functions and clean them up in `finally`.

These helpers are useful in Blazor WebAssembly scenarios when you want to route the same high-level library API through JS interop.

## Asymmetric encryption helpers

## RsaExtensions

### Method family

- `CreateRsaKeyPair(...)`
- `GetRsaKeyEncoding(...)`
- `ValidateRsaPublicKey(...)`
- `ValidateRsaPrivateKey(...)`
- `EncryptRsa(...)`
- `EncryptRsaEncoded(...)`
- `DecryptRsa(...)`
- `DecryptRsaFromEncoded(...)`
- async `IJSRuntime` variants for generation, validation, detection, encryption, and decryption

### Supported key formats

- BER
- PEM
- XML

The detection path validates these formats with source-generated regular expressions before importing the key material. Public behavior stays the same, but the implementation avoids rebuilding the regex objects at runtime for repeated RSA key checks.

### Key generation example

```csharp
RsaKeyPair pair = 4096u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);
```

### Direct encryption example

```csharp
RsaKeyPair pair = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);

string encrypted = "payload".EncryptRsaEncoded(
    pair.PublicKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256);

string decrypted = encrypted.DecryptRsaFromEncoded(
    pair.PrivateKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256).ToStringUtf8();
```

### IJSRuntime example

```csharp
string encrypted = await jsRuntime.EncryptRsaEncodedAsync(
    "payload",
    publicKey,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256);

string decrypted = (await jsRuntime.DecryptRsaFromEncodedAsync(
    encrypted,
    privateKey,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256)).ToStringUtf8();
```

## Random helpers

## RandomExtensions

### Method family

- `NextValue(this int value)`
- `NextValue(this int min, int max)`
- `NextBytes(this uint size)`

### Usage guidance

- use `NextBytes(...)` when you need raw random material
- use `NextValue(min, max)` for bounded integer generation, noting that it rejects `min >= max`

## Shared utility pieces relevant to crypto callers

### TypeIdentifierAttribute

Decorates concrete serializable types with a stable identifier string.

### TypeIdentifierResolver

Used by concrete serializers and dynamic contract deserializers to:

- write `TypeIdentifier`
- validate a payload against a concrete type
- resolve a concrete implementation by identifier

### EncodingExtensions

Common UTF-8 helpers used by almost every cryptography string overload:

- `ToByteArrayUtf8`
- `ToStringUtf8`

### ByteArrayExtensions

Basic guards:

- `ThrowIfNull`
- `ThrowIfEmpty`
- `ThrowIfNullOrEmpty`

## Recommended usage patterns

### Prefer extension methods when

- you only need one operation
- you do not need serialization
- you want explicit algorithm choice inline

### Prefer concrete classes when

- you want reusable config
- you want object serialization
- you want dependency injection
- you want dynamic deserialization from stored JSON

### Prefer dynamic deserialization when

- the JSON may represent multiple concrete implementations
- you are reading persisted configuration from storage
- you are building plug-in or user-selectable cryptography flows

## Security notes

- AES-CBC encrypts but does not authenticate
- MD5 and SHA-1 remain available mainly for compatibility
- prefer OAEP padding for RSA unless compatibility forces PKCS#1 v1.5
- treat serialized keys, salts, and HMAC keys as sensitive material
