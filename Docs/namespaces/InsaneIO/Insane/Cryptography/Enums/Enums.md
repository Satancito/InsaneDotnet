# InsaneIO.Insane.Cryptography.Enums

Parent namespace: [InsaneIO.Insane.Cryptography](../Cryptography.md)

This namespace contains the enums that shape the public cryptography surface.

Use these enums to configure the concrete cryptography classes and extension helpers without relying on ad hoc magic values.

## Public enums

- `AesCbcPadding`
- `Argon2Variant`
- `Base64Encoding`
- `EncoderType`
- `HashAlgorithm`
- `RsaKeyEncoding`
- `RsaKeyPairEncoding`
- `RsaPadding`

## AesCbcPadding

Controls the `PaddingMode` used by the AES-CBC helpers and `AesCbcEncryptor`.

- `None`
- `Zeros`
- `Pkcs7`
- `AnsiX923`

## Argon2Variant

Selects the Argon2 algorithm family:

- `Argon2d`
- `Argon2i`
- `Argon2id`

`Argon2id` is the default in `Argon2Hasher`.

## Base64Encoding

Controls how `Base64Encoder` writes text:

- `Base64`
- `UrlSafeBase64`
- `FileNameSafeBase64`
- `UrlEncodedBase64`

## EncoderType

Represents the logical encoder families exposed by the package:

- `Hex`
- `Base32`
- `Base64`

## HashAlgorithm

Controls hash and HMAC selection across the package:

- `Md5`
- `Sha1`
- `Sha256`
- `Sha384`
- `Sha512`

For TOTP specifically, `Md5` and `Sha384` are normalized to `Sha1`.

## RsaKeyEncoding

Represents the detected format of a single RSA key:

- `Unknown`
- `BerPublic`
- `BerPrivate`
- `PemPublic`
- `PemPrivate`
- `XmlPublic`
- `XmlPrivate`

## RsaKeyPairEncoding

Controls the output format for `CreateRsaKeyPair(...)`:

- `Ber`
- `Pem`
- `Xml`

## RsaPadding

Controls RSA encryption padding:

- `Pkcs1`
- `OaepSha1`
- `OaepSha256`
- `OaepSha384`
- `OaepSha512`
