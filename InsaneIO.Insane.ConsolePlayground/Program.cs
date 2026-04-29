using InsaneIO.Insane.Cryptography;
using InsaneIO.Insane.Cryptography.Abstractions;
using InsaneIO.Insane.Cryptography.Enums;
using InsaneIO.Insane.Cryptography.Extensions;
using InsaneIO.Insane.Extensions;

const string publicKey = "-----BEGIN PUBLIC KEY-----\r\nMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA015fxIM7nABLMtYmwfSz\r\ngsux7ktrhghkW65ZXqgEqEbf2alOfiaDMAdO3CvJ7ifWNunxH//2ONgzRImwCaUv\r\n5puFilhz4nNt0Z9UwMzXLOMbtDGVUnv57eFjRFLsP9eU58z8tbRBHKLYKZfWmfaf\r\nBGFPFS3IaRc5qEF11KFAAN/NOntYmrEe8hhDFn/PniulFWevAG8t+/aO7N+4xquG\r\nLdjE0mlSJYvFX/yESbKiBitxvODx1kV4oYH4qDvOCVTYdcgBd9EDps6h2i/y+BYx\r\n6dY9/IRo3iFKWNUeccuNf9ZAKIpN5eFsnymAV1LM7o3YqN2Mt397b3pW6FUIrxye\r\n8QIDAQAB\r\n-----END PUBLIC KEY-----";

const string privateKey = "-----BEGIN PRIVATE KEY-----\r\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQDTXl/EgzucAEsy\r\n1ibB9LOCy7HuS2uGCGRbrlleqASoRt/ZqU5+JoMwB07cK8nuJ9Y26fEf//Y42DNE\r\nibAJpS/mm4WKWHPic23Rn1TAzNcs4xu0MZVSe/nt4WNEUuw/15TnzPy1tEEcotgp\r\nl9aZ9p8EYU8VLchpFzmoQXXUoUAA3806e1iasR7yGEMWf8+eK6UVZ68Aby379o7s\r\n37jGq4Yt2MTSaVIli8Vf/IRJsqIGK3G84PHWRXihgfioO84JVNh1yAF30QOmzqHa\r\nL/L4FjHp1j38hGjeIUpY1R5xy41/1kAoik3l4WyfKYBXUszujdio3Yy3f3tvelbo\r\nVQivHJ7xAgMBAAECggEBAKp5LjI3ScdrMTtCHyZFbtap5Mr1hzYl5JNRDgFa786s\r\nwvQxKf5yn1IIQuEubAGpnYrSqcgOUA7OyKKZjiJpWTXb7xuCdYyJXmQ35kpNq6on\r\nTXvKlc1d8liadM5xNtvSyuUdniAKzo0DBead8NZiYyRMO1UwhLfFpJoAmcExgIZy\r\ngz+bftNZJQhU2BzECe1T98tR65Zjy84jp+TO2z8isj8Fte/WqcP+dO8gwToRJZ2u\r\n71dp86nsG5/XnhB790TcoN09pWcqLzpkkgW8asCsLe3YV088kujSkat2hgqUOMJ3\r\nLmyZh1TXNHTudClgsxbpCt6wvrDKTD/xrIFPFJ23sIUCgYEA54EAy/LuwL+IoEJ/\r\nlynkVkRwgrKxODcYINSIAwdbfTNyYxDa6+b+wuaGo5ENU10j3E82CP7/YEQkaW4Q\r\nSsA3vhqs2Fy2bvz8J9mRJ/fDVBdcGf60COogbYpw3y54Y7tbREmlyHbRir7k6Du3\r\nJomXIRpWfsqWIBJyfgJ92wpf8scCgYEA6bvx9nFF0513hPJ21pR1KIxd9QQzPk6J\r\nRB3IZdGwL7dCETv58OOZJa/bSMFaIvmUeo+n+p8BhsvJI2p1A+nkEy8D9VAhOoXR\r\ngYNIzHB0gT0FyRNuE0wINlLvoPXtqBOIRjh2zWwV71ltYjVDbsHHfdJTSHGkG5nL\r\nZXpvA8ZHqIcCgYATHEEWlO1EmAsNP7AMqHai8G9hOqMKgvHI9AJuTZMt2KtepiuM\r\nQbtSF3mR0w2Iji3BqABL6dDm/3kKAv3xTcDMPqN6EN02eEfQelNTO06yiGMf+jie\r\na8VVhZIfulRR10uHbZTz4WsWyv9WdGeAFOeW4fe1679M0nkFEeuVw1t8TQKBgQCM\r\na/KxGVpXCTp97+E/h09fvfzQr+ZNX4eOGcqEiVatRA74Ja1CcKpYfu9iJW+OpZzz\r\niQzlP9P99L5xfYqxgDoa4FsjbTGZZD367jG1STZlUpEAPaEbDMm+QVNfw4A1Qw7T\r\nCScuwOnoszRJFTDfAKJmUA7i7gsm3d3ZqJW7kcsJTwKBgQDluZVK8bWHVMnMkTpk\r\nyZn1PnIefNfIFcemY5rzq1Ty28XMe3ayvRIzga14/C6CKyEUZ75ianyQ3SHiPVrS\r\nta3v3oVYrBsxDevjqQFWGimfOWgN7HF52zy0dYUs3eXiRBhe0ybCvt5URgCoV6eg\r\n3LlwGDbVYRci/r+P1BTNWLIODA==\r\n-----END PRIVATE KEY-----";

const string data = "Hello from InsaneIO.Insane.ConsolePlayground";

IEncoder encoder = Base64Encoder.DefaultInstance;
RsaKeyPair keyPair = new()
{
    PublicKey = publicKey,
    PrivateKey = privateKey
};

Console.WriteLine($"Public key valid: {publicKey.ValidateRsaPublicKey()}");
Console.WriteLine($"Private key valid: {privateKey.ValidateRsaPrivateKey()}");
Console.WriteLine($"Public key encoding: {publicKey.GetRsaKeyEncoding()}");
Console.WriteLine($"Private key encoding: {privateKey.GetRsaKeyEncoding()}");
Console.WriteLine();

string encrypted = data.EncryptRsaEncoded(publicKey, encoder, RsaPadding.OaepSha256);
string decrypted = encrypted.DecryptRsaFromEncoded(privateKey, encoder, RsaPadding.OaepSha256).ToStringUtf8();

Console.WriteLine($"Original:  {data}");
Console.WriteLine($"Encrypted: {encrypted}");
Console.WriteLine($"Decrypted: {decrypted}");
Console.WriteLine($"Round trip ok: {decrypted == data}");
Console.WriteLine();

IEncryptor encryptor = new RsaEncryptor
{
    KeyPair = keyPair,
    Padding = RsaPadding.OaepSha256,
    Encoder = encoder
};

string wrappedEncrypted = encryptor.EncryptEncoded(data);
string wrappedDecrypted = encryptor.DecryptEncoded(wrappedEncrypted).ToStringUtf8();

Console.WriteLine($"Encryptor round trip ok: {wrappedDecrypted == data}");
Console.WriteLine();
Console.WriteLine("Press ENTER to exit...");
Console.ReadLine();
