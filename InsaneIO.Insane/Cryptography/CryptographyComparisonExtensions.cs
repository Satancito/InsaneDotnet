using System.Security.Cryptography;

namespace InsaneIO.Insane.Cryptography.Extensions
{
    internal static class CryptographyComparisonExtensions
    {
        internal static bool FixedTimeEquals(byte[] actual, byte[] expected)
        {
            ArgumentNullException.ThrowIfNull(actual);
            ArgumentNullException.ThrowIfNull(expected);
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        internal static bool FixedTimeEqualsEncoded(string actual, string expected)
        {
            ArgumentNullException.ThrowIfNull(actual);
            ArgumentNullException.ThrowIfNull(expected);
            return CryptographicOperations.FixedTimeEquals(actual.ToByteArrayUtf8(), expected.ToByteArrayUtf8());
        }
    }
}

