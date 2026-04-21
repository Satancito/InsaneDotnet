using System.Reflection.Emit;
using System.Security.Cryptography;


namespace InsaneIO.Insane.Cryptography
{
    /// <summary>
    /// 
    /// </summary>
    public static class RandomExtensions
    {
        public static int NextValue(this int value)
        {
            byte[] intBytes = new byte[4];
            RandomNumberGenerator.Fill(intBytes);
            return BitConverter.ToInt32(intBytes, 0) ^ value;
        }

        public static int NextValue(this int min, int max)
        {
            if (min >= max)
            {
                throw new ArgumentException("Min value is greater or equals than Max value.");
            }
            return min + (Math.Abs(0.NextValue()) % (max - min + 1));
        }

        public static byte[] NextBytes(this uint size)
        {
            byte[] ret = new byte[size];
            RandomNumberGenerator.Fill(ret);
            return ret;
        }
    }
}
