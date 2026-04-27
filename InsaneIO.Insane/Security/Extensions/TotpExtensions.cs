

using System.Runtime.Versioning;

namespace InsaneIO.Insane.Extensions
{
    
    public static class TotpExtensions
    {
        private const uint InitialCounterTime = 0;
        public const uint TotpDefaultPeriod = 30;

        public static string GenerateTotpUri(this byte[] secret, string label, string issuer, HashAlgorithm algorithm = HashAlgorithm.Sha1, TwoFactorCodeLength codeLength = TwoFactorCodeLength.SixDigits, long timePeriodInSeconds = TotpDefaultPeriod)
        {
            return $"otpauth://totp/{HttpUtility.UrlEncode(label)}?secret={secret.EncodeToBase32(true)}&issuer={HttpUtility.UrlEncode(issuer)}&algorithm={algorithm.ToString().ToUpper()}&digits={codeLength.IntValue()}&period={timePeriodInSeconds}";
        }

        public static string GenerateTotpUri(this string base32EncodedSecret, string label, string issuer)
        {
            return GenerateTotpUri(Base32Encoder.DefaultInstance.Decode(base32EncodedSecret), label, issuer);
        }

        public static string ComputeTotpCode(this byte[] secret, DateTimeOffset now, TwoFactorCodeLength length = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, long timePeriodInSeconds = TotpDefaultPeriod)
        {
            hashAlgorithm = hashAlgorithm switch
            {
                HashAlgorithm.Md5 => HashAlgorithm.Sha1,
                _ => hashAlgorithm
            };
            long timeInterval = (now.ToUnixTimeSeconds() - DateTimeOffset.UnixEpoch.ToUnixTimeSeconds()) / timePeriodInSeconds;
            timeInterval = BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(timeInterval) : timeInterval;
            byte[] hmac = BitConverter.GetBytes(timeInterval).ComputeHmac(secret, hashAlgorithm);
            byte offset = (byte)(hmac[Constants.Sha1HashSizeInBytes - 1] & 0x0F);
            var slice = hmac[offset..(offset + 4)];
            long code = ((BitConverter.IsLittleEndian ? BinaryPrimitives.ReadInt32BigEndian(slice) : BinaryPrimitives.ReadInt32LittleEndian(slice)) & 0x7FFFFFFF) % (int)(Math.Pow(10, length.IntValue()));
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

        public static bool VerifyTotpCode(this string code, byte[] secret, TwoFactorCodeLength length = TwoFactorCodeLength.SixDigits, HashAlgorithm hashAlgorithm = HashAlgorithm.Sha1, long timePeriodInSeconds = TotpDefaultPeriod)
        {
            return VerifyTotpCode(code, secret, DateTimeOffset.UtcNow, length, hashAlgorithm, timePeriodInSeconds);
        }

        public static bool VerifyTotpCode(this string code, string base32EncodedSecret)
        {
            return VerifyTotpCode(code, Base32Encoder.DefaultInstance.Decode(base32EncodedSecret));
        }

        public static long ComputeTotpRemainingSeconds(this DateTimeOffset now, long timePeriodInSeconds = TotpDefaultPeriod)
        {
            return timePeriodInSeconds - (now.ToUniversalTime().ToUnixTimeSeconds() - DateTimeOffset.UnixEpoch.ToUnixTimeSeconds()) % timePeriodInSeconds;
        }

    }
}
