using System.Security.Cryptography;
using HashAlgorithm = InsaneIO.Insane.Cryptography.Enums.HashAlgorithm;

namespace InsaneIO.Insane.Cryptography.Extensions;

public static class HmacExtensions
{
    public static byte[] ComputeHmac(this byte[] data, byte[] key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        switch (algorithm)
        {
            case HashAlgorithm.Md5:
                using (HMACMD5 hmac = new(key))
                {
                    return hmac.ComputeHash(data);
                }
            case HashAlgorithm.Sha1:
                using (HMACSHA1 hmac = new(key))
                {
                    return hmac.ComputeHash(data);
                }
            case HashAlgorithm.Sha256:
                using (HMACSHA256 hmac = new(key))
                {
                    return hmac.ComputeHash(data);
                }
            case HashAlgorithm.Sha384:
                using (HMACSHA384 hmac = new(key))
                {
                    return hmac.ComputeHash(data);
                }
            case HashAlgorithm.Sha512:
                using (HMACSHA512 hmac = new(key))
                {
                    return hmac.ComputeHash(data);
                }
            default:
                throw new NotImplementedException(algorithm.ToString());
        }
    }

    public static byte[] ComputeHmac(this string data, byte[] key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return ComputeHmac(data.ToByteArrayUtf8(), key, algorithm);
    }

    public static byte[] ComputeHmac(this string data, string key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return ComputeHmac(data.ToByteArrayUtf8(), key.ToByteArrayUtf8(), algorithm);
    }

    public static byte[] ComputeHmac(this byte[] data, string key, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return ComputeHmac(data, key.ToByteArrayUtf8(), algorithm);
    }

    public static string ComputeHmacEncoded(this byte[] data, byte[] key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return encoder.Encode(ComputeHmac(data, key, algorithm));
    }

    public static string ComputeHmacEncoded(this string data, string key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return encoder.Encode(ComputeHmac(data, key, algorithm));
    }

    public static string ComputeHmacEncoded(this byte[] data, string key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return encoder.Encode(ComputeHmac(data, key, algorithm));
    }

    public static string ComputeHmacEncoded(this string data, byte[] key, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return encoder.Encode(ComputeHmac(data, key, algorithm));
    }

    public static bool VerifyHmac(this byte[] data, byte[] key, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeHmac(key, algorithm), expected);
    }

    public static bool VerifyHmac(this string data, byte[] key, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeHmac(key, algorithm), expected);
    }

    public static bool VerifyHmac(this string data, string key, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeHmac(key, algorithm), expected);
    }

    public static bool VerifyHmac(this byte[] data, string key, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeHmac(key, algorithm), expected);
    }

    public static bool VerifyHmacFromEncoded(this byte[] data, byte[] key, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeHmacEncoded(key, encoder, algorithm), expected);
    }

    public static bool VerifyHmacFromEncoded(this string data, string key, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeHmacEncoded(key, encoder, algorithm), expected);
    }

    public static bool VerifyHmacFromEncoded(this string data, byte[] key, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeHmacEncoded(key, encoder, algorithm), expected);
    }

    public static bool VerifyHmacFromEncoded(this byte[] data, string key, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
    {
        return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeHmacEncoded(key, encoder, algorithm), expected);
    }
}

