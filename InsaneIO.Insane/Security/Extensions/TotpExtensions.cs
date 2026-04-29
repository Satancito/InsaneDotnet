

using System.Runtime.Versioning;
using InsaneIO.Insane.Cryptography.Enums;
using InsaneIO.Insane.Security.Enums;

namespace InsaneIO.Insane.Security.Extensions;

public static class TotpExtensions
{
    private const uint InitialCounterTime = 0;
    public const uint TotpDefaultPeriod = 30;

    private static HashAlgorithm NormalizeTotpHashAlgorithm(HashAlgorithm hashAlgorithm)
    {
        return hashAlgorithm switch
        {
            HashAlgorithm.Md5 => HashAlgorithm.Sha1,
            HashAlgorithm.Sha1 => HashAlgorithm.Sha1,
            HashAlgorithm.Sha256 => HashAlgorithm.Sha256,
            HashAlgorithm.Sha384 => HashAlgorithm.Sha1,
            HashAlgorithm.Sha512 => HashAlgorithm.Sha512,
            _ => throw new NotSupportedException($"Hash algorithm '{hashAlgorithm}' is not supported for TOTP.")
        };
    }

    private static string GetTotpAlgorithmName(HashAlgorithm hashAlgorithm)
    {
        HashAlgorithm normalized = NormalizeTotpHashAlgorithm(hashAlgorithm);
        return normalized switch
        {
            HashAlgorithm.Sha1 => "SHA1",
            HashAlgorithm.Sha256 => "SHA256",
            HashAlgorithm.Sha512 => "SHA512",
            _ => throw new NotSupportedException($"Hash algorithm '{normalized}' is not supported for TOTP.")
        };
    }

    public static string GenerateTotpUri(this byte[] secret, string label, string issuer, HashAlgorithm algorithm = HashAlgorithm.Sha1, TwoFactorCodeLength codeLength = TwoFactorCodeLength.SixDigits, long timePeriodInSeconds = TotpDefaultPeriod)
    {
        return $"otpauth://totp/{HttpUtility.UrlEncode(label)}?secret={secret.EncodeToBase32(true)}&issuer={HttpUtility.UrlEncode(issuer)}&algorithm={GetTotpAlgorithmName(algorithm)}&digits={codeLength.IntValue()}&period={timePeriodInSeconds}";
    }

    public static string GenerateTotpUri(this string base32EncodedSecret, string label, string issuer)
    {
        return GenerateTotpUri(Base32Encoder.DefaultInstance.Decode(base32EncodedSecret), label, issuer);
    }

    public static string ComputeTotpCode(this byte[] secret, DateTimeOffset now, TwoFactorCodeLength length = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, long timePeriodInSeconds = TotpDefaultPeriod)
    {
        hashAlgorithm = NormalizeTotpHashAlgorithm(hashAlgorithm);
        long timeInterval = (now.ToUnixTimeSeconds() - DateTimeOffset.UnixEpoch.ToUnixTimeSeconds()) / timePeriodInSeconds;
        byte[] counterBytes = new byte[sizeof(long)];
        BinaryPrimitives.WriteInt64BigEndian(counterBytes, timeInterval);
        byte[] hmac = counterBytes.ComputeHmac(secret, hashAlgorithm);
        byte offset = (byte)(hmac[hmac.Length - 1] & 0x0F);
        var slice = hmac[offset..(offset + 4)];
        long code = (BinaryPrimitives.ReadInt32BigEndian(slice) & 0x7FFFFFFF) % (int)Math.Pow(10, length.IntValue());
        return code.ToString().PadLeft(length.IntValue(), '0');
    }

    public static string ComputeTotpCode(this byte[] secret, TwoFactorCodeLength length = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, long timePeriodInSeconds = TotpDefaultPeriod)
    {
        return ComputeTotpCode(secret, DateTimeOffset.UtcNow, length, hashAlgorithm, timePeriodInSeconds);
    }

    public static string ComputeTotpCode(this string base32EncodedSecret)
    {
        return ComputeTotpCode(Base32Encoder.DefaultInstance.Decode(base32EncodedSecret));
    }

    public static bool VerifyTotpCode(this string code, byte[] secret, DateTimeOffset now, TwoFactorCodeLength length = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, long timePeriodInSeconds = TotpDefaultPeriod)
    {
        return ComputeTotpCode(secret, now, length, hashAlgorithm, timePeriodInSeconds).Equals(code);
    }

    public static bool VerifyTotpCode(this string code, byte[] secret, DateTimeOffset now, TotpTimeWindowTolerance tolerance, TwoFactorCodeLength length = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, long timePeriodInSeconds = TotpDefaultPeriod)
    {
        int windowCount = (int)tolerance;
        if (windowCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance));
        }

        for (int offset = -windowCount; offset <= windowCount; offset++)
        {
            if (code.VerifyTotpCode(secret, now.AddSeconds(offset * timePeriodInSeconds), length, hashAlgorithm, timePeriodInSeconds))
            {
                return true;
            }
        }

        return false;
    }

    public static bool VerifyTotpCode(this string code, byte[] secret, TwoFactorCodeLength length = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, long timePeriodInSeconds = TotpDefaultPeriod)
    {
        return VerifyTotpCode(code, secret, DateTimeOffset.UtcNow, length, hashAlgorithm, timePeriodInSeconds);
    }

    public static bool VerifyTotpCode(this string code, byte[] secret, TotpTimeWindowTolerance tolerance, TwoFactorCodeLength length = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, long timePeriodInSeconds = TotpDefaultPeriod)
    {
        return VerifyTotpCode(code, secret, DateTimeOffset.UtcNow, tolerance, length, hashAlgorithm, timePeriodInSeconds);
    }

    public static bool VerifyTotpCode(this string code, string base32EncodedSecret)
    {
        return VerifyTotpCode(code, Base32Encoder.DefaultInstance.Decode(base32EncodedSecret));
    }

    public static bool VerifyTotpCode(this string code, string base32EncodedSecret, TotpTimeWindowTolerance tolerance)
    {
        return VerifyTotpCode(code, Base32Encoder.DefaultInstance.Decode(base32EncodedSecret), tolerance);
    }

    public static long ComputeTotpRemainingSeconds(this DateTimeOffset now, long timePeriodInSeconds = TotpDefaultPeriod)
    {
        return timePeriodInSeconds - (now.ToUniversalTime().ToUnixTimeSeconds() - DateTimeOffset.UnixEpoch.ToUnixTimeSeconds()) % timePeriodInSeconds;
    }

}
