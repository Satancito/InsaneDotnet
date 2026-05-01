# InsaneIO.Insane.Cryptography.Extensions

Parent namespace: [InsaneIO.Insane.Cryptography](../Cryptography.md)

This namespace contains the direct helper API for encoding, hashing, key derivation, symmetric encryption, asymmetric encryption, and cryptographic randomness.

Prefer this namespace when you want one-off operations without building a stateful object first.

## Public extension classes

- `Base32EncodingExtensions`
- `Base64EncodingExtensions`
- `HexEncodingExtensions`
- `HashExtensions`
- `HmacExtensions`
- `ScryptExtensions`
- `Argon2Extensions`
- `AesExtensions`
- `RsaExtensions`
- `RandomExtensions`

## Base32EncodingExtensions

### What it does

- `EncodeToBase32(...)` for `byte[]` and `string`
- `DecodeFromBase32(string data)`

### Behavior notes

- `string` inputs are converted with UTF-8
- decoding accepts upper and lower case
- decoding trims the outer input
- invalid lengths, invalid padding placement, invalid padding lengths, and invalid characters throw `ArgumentException`

### Example

```csharp
byte[] secret = "hello".ToByteArrayUtf8();
string padded = secret.EncodeToBase32();
string compact = secret.EncodeToBase32(removePadding: true, toLower: true);
byte[] roundTrip = compact.DecodeFromBase32();
```

## Base64EncodingExtensions

### Method families

- `InsertLineBreaks(...)`
- `EncodeToBase64(...)`
- `EncodeToUrlSafeBase64(...)`
- `EncodeToFilenameSafeBase64(...)`
- `EncodeToUrlEncodedBase64(...)`
- `DecodeFromBase64(...)`
- conversion helpers from already encoded Base64 text

### Overload behavior

- `EncodeToBase64(string data, ...)` uses UTF-8
- `DecodeFromBase64(...)` accepts:
  - standard Base64
  - URL-safe Base64
  - filename-safe Base64
  - URL-encoded Base64
  - values containing `\r` / `\n`

### Example

```csharp
byte[] data = { 0xfb, 0xff, 0xee };

string standard = data.EncodeToBase64();
string urlSafe = data.EncodeToUrlSafeBase64();
string urlEncoded = data.EncodeToUrlEncodedBase64();

byte[] decoded = urlSafe.DecodeFromBase64();
```

## HexEncodingExtensions

### Method families

- `EncodeToHex(byte[] data, bool toUpper = false)`
- `EncodeToHex(string data, bool toUpper = false)`
- `DecodeFromHex(string data)`

### Failure behavior

- `null` throws `ArgumentNullException`
- odd-length input throws `ArgumentException`
- invalid characters throw `ArgumentException`

## HashExtensions

### Method families

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

### Important behavior

- `string` overloads always use UTF-8
- encoded verification uses the provided `IEncoder`
- verification methods use fixed-time comparison helpers

## HmacExtensions

### Method families

- `ComputeHmac(...)`
- `ComputeHmacEncoded(...)`
- `VerifyHmac(...)`
- `VerifyHmacFromEncoded(...)`

### Overload matrix

The helpers accept both:

- `byte[]` data and `byte[]` key
- `string` data and/or `string` key

String values are converted with UTF-8 before hashing.

## ScryptExtensions

### Method families

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

### Example

```csharp
string encoded = "password".ComputeScryptEncoded(
    salt: "salt",
    encoder: Base64Encoder.DefaultInstance,
    iterations: 16384,
    blockSize: 8,
    parallelism: 1,
    derivedKeyLength: 64);
```

## Argon2Extensions

### Method families

- `ComputeArgon2(...)`
- `ComputeArgon2Encoded(...)`
- `VerifyArgon2(...)`
- `VerifyArgon2FromEncoded(...)`

### Variant behavior

The variant is controlled by `Argon2Variant` and maps to:

- `Argon2d`
- `Argon2i`
- `Argon2id`

### Example

```csharp
string encoded = "password".ComputeArgon2Encoded(
    salt: "salt",
    encoder: Base64Encoder.DefaultInstance,
    iterations: 2,
    memorySizeKiB: 16384,
    parallelism: 4,
    variant: Argon2Variant.Argon2id,
    derivedKeyLength: 64);
```

## AesExtensions

### Method families

- `EncryptAesCbc(...)`
- `DecryptAesCbc(...)`
- `EncryptAesCbcEncoded(...)`
- `DecryptAesCbcFromEncoded(...)`
- `IJSRuntime` async counterparts for the same operations

### Key handling

- the public API accepts either `byte[]` or `string` keys
- keys shorter than 8 bytes are rejected
- the implementation hashes the supplied key with SHA-512 and uses the first 32 bytes as the AES key

### Ciphertext layout

The returned byte array appends the IV to the end of the ciphertext. Decryption expects the last 16 bytes to contain that IV.

### Validation

- `DecryptAesCbc(...)` rejects ciphertext shorter than the IV length
- unsupported padding values throw `NotImplementedException`

### Example

```csharp
string cipher = "payload".EncryptAesCbcEncoded(
    key: "secret-password",
    encoder: Base64Encoder.DefaultInstance,
    padding: AesCbcPadding.Pkcs7);

string clear = cipher.DecryptAesCbcFromEncoded(
    key: "secret-password",
    encoder: Base64Encoder.DefaultInstance,
    padding: AesCbcPadding.Pkcs7).ToStringUtf8();
```

## RsaExtensions

### Method families

- key generation: `CreateRsaKeyPair(...)`
- key validation: `ValidateRsaPublicKey(...)`, `ValidateRsaPrivateKey(...)`
- key detection: `GetRsaKeyEncoding(...)`
- encryption: `EncryptRsa(...)`, `EncryptRsaEncoded(...)`
- decryption: `DecryptRsa(...)`, `DecryptRsaFromEncoded(...)`
- `IJSRuntime` async counterparts for generation, validation, detection, encryption, and decryption

### Supported key formats

- BER / Base64 body
- PEM
- XML

The current implementation validates these formats with source-generated regular expressions before attempting the RSA import step. That keeps key detection predictable while reducing repeated regex setup work in repeated validation flows.

### Key generation

```csharp
RsaKeyPair pair = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);
```

### Round-trip example

```csharp
RsaKeyPair pair = 2048u.CreateRsaKeyPair(RsaKeyPairEncoding.Pem);

string encrypted = "payload".EncryptRsaEncoded(
    pair.PublicKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256);

string clear = encrypted.DecryptRsaFromEncoded(
    pair.PrivateKey!,
    Base64Encoder.DefaultInstance,
    RsaPadding.OaepSha256).ToStringUtf8();
```

### Validation and failure behavior

- invalid keys return `false` from the validation helpers
- invalid keys passed into encryption/decryption throw `ArgumentException`

## RandomExtensions

### Method families

- `NextValue(this int value)`
- `NextValue(this int min, int max)`
- `NextBytes(this uint size)`

### Usage guidance

These helpers expose cryptographic randomness in a lightweight form.

- `NextBytes(...)` is the most generally useful API
- `NextValue(min, max)` throws when `min >= max`

```csharp
byte[] nonce = 16u.NextBytes();
```
