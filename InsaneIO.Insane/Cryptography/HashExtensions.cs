using System.Security.Cryptography;
using HashAlgorithm = InsaneIO.Insane.Cryptography.HashAlgorithm;

namespace InsaneIO.Insane.Cryptography.Extensions
{
    public static class HashExtensions
    {
        public static byte[] ComputeHash(this byte[] data, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            switch (algorithm)
            {
                case HashAlgorithm.Md5:
                    using (MD5 md5 = MD5.Create())
                    {
                        return md5.ComputeHash(data);
                    }
                case HashAlgorithm.Sha1:
                    using (SHA1 sha1 = SHA1.Create())
                    {
                        return sha1.ComputeHash(data);
                    }
                case HashAlgorithm.Sha256:
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        return sha256.ComputeHash(data);
                    }
                case HashAlgorithm.Sha384:
                    using (SHA384 sha384 = SHA384.Create())
                    {
                        return sha384.ComputeHash(data);
                    }
                case HashAlgorithm.Sha512:
                    using (SHA512 sha512 = SHA512.Create())
                    {
                        return sha512.ComputeHash(data);
                    }
                default:
                    throw new NotImplementedException(algorithm.ToString());
            }
        }

        public static byte[] ComputeHash(this string data, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return ComputeHash(data.ToByteArrayUtf8(), algorithm);
        }

        public static string ComputeHashEncoded(this byte[] data, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return encoder.Encode(ComputeHash(data, algorithm));
        }

        public static string ComputeHashEncoded(this string data, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return encoder.Encode(ComputeHash(data, algorithm));
        }

        public static bool VerifyHash(this byte[] data, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeHash(algorithm), expected);
        }

        public static bool VerifyHash(this string data, byte[] expected, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return CryptographyComparisonExtensions.FixedTimeEquals(data.ComputeHash(algorithm), expected);
        }

        public static bool VerifyHashFromEncoded(this byte[] data, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeHashEncoded(encoder, algorithm), expected);
        }

        public static bool VerifyHashFromEncoded(this string data, string expected, IEncoder encoder, HashAlgorithm algorithm = HashAlgorithm.Sha512)
        {
            return CryptographyComparisonExtensions.FixedTimeEqualsEncoded(data.ComputeHashEncoded(encoder, algorithm), expected);
        }
    }
}

