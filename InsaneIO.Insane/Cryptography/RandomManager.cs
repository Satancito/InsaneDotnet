﻿using System.Security.Cryptography;


namespace InsaneIO.Insane.Cryptography
{
    /// <summary>
    /// 
    /// </summary>
    public static class RandomManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int Next()
        {
            byte[] intBytes = new byte[4];
            RandomNumberGenerator.Fill(intBytes);
            return BitConverter.ToInt32(intBytes, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Next(int min, int max)
        {
            if (min >= max)
            {
                throw new ArgumentException("Min value is greater or equals than Max value.");
            }
            byte[] intBytes = new byte[4];
            RandomNumberGenerator.Fill(intBytes);
            return min + Math.Abs(BitConverter.ToInt32(intBytes, 0)) % (max - min + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] Next(uint size)
        {
            byte[] ret = new byte[size];
            RandomNumberGenerator.Fill(ret);
            return ret;
        }
    }
}