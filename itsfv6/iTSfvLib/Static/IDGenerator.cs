using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iTSfvLib
{
    public static class IDGenerator
    {
        public static readonly Random Random = new Random();

        public const string Alphabet = "abcdefghijklmnopqrstuvwxyz";
        public const string AlphabetCapital = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string Numbers = "0123456789";
        public const string Alphanumeric = Alphabet + AlphabetCapital + Numbers;

        public static string GetRandomAlphanumeric()
        {
            return GetRandomAlphanumeric(12);
        }

        /// <summary>abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789</summary>
        private static string GetRandomAlphanumeric(int length)
        {
            return GetRandomString(Alphanumeric, length);
        }

        private static string GetRandomString(string chars, int length)
        {
            StringBuilder sb = new StringBuilder();

            while (length-- > 0)
            {
                sb.Append(GetRandomChar(chars));
            }

            return sb.ToString();
        }

        private static char GetRandomChar(string chars)
        {
            return chars[Random.Next(chars.Length)];
        }
    }
}